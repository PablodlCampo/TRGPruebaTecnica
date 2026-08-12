using System;
using System.Linq;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Api.Models;
using GtMotive.Estimate.Microservice.Api.Presenters;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.Controllers
{
    /// <summary>
    /// Provides REST endpoints for managing vehicles.
    /// </summary>
    [ApiController]
    [Route("vehicles")]
#pragma warning disable S6960 // Controllers should not have mixed responsibilities
    public sealed class VehiclesController : ControllerBase
#pragma warning restore S6960 // Controllers should not have mixed responsibilities
    {
        private readonly IUseCase<CreateVehicleInput> _createVehicleUseCase;
        private readonly IUseCase _listVehiclesUseCase;
        private readonly CreateVehiclePresenter _createVehiclePresenter;
        private readonly ListVehiclesPresenter _listVehiclesPresenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="VehiclesController"/> class.
        /// </summary>
        /// <param name="createVehicleUseCase">
        /// Use case responsible for creating vehicles.
        /// </param>
        /// <param name="listVehiclesUseCase">
        /// Use case responsible for retrieving available vehicles.
        /// </param>
        /// <param name="createVehiclePresenter">
        /// Presenter used to format the API responses.
        /// </param>
        /// <param name="listVehiclesPresenter">
        /// Presenter used to format the API responses for listing vehicles.
        /// </param>
        /// <returns>
        /// A new instance of the <see cref="VehiclesController"/> class.
        /// </returns>
        public VehiclesController(
            IUseCase<CreateVehicleInput> createVehicleUseCase,
            IUseCase listVehiclesUseCase,
            CreateVehiclePresenter createVehiclePresenter,
            ListVehiclesPresenter listVehiclesPresenter)
        {
            ArgumentNullException.ThrowIfNull(createVehicleUseCase);
            ArgumentNullException.ThrowIfNull(listVehiclesUseCase);
            ArgumentNullException.ThrowIfNull(createVehiclePresenter);
            ArgumentNullException.ThrowIfNull(listVehiclesPresenter);

            _createVehicleUseCase = createVehicleUseCase;
            _listVehiclesUseCase = listVehiclesUseCase;
            _createVehiclePresenter = createVehiclePresenter;
            _listVehiclesPresenter = listVehiclesPresenter;
        }

        /// <summary>
        /// Creates a new vehicle in the fleet.
        /// </summary>
        /// <param name="request">
        /// Request containing the vehicle registration number and manufacturing date.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        [HttpPost]
        [ProducesResponseType(
            typeof(CreateVehicleResponse),
            StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] CreateVehicleRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            await _createVehicleUseCase.Execute(new CreateVehicleInput(request.RegistrationNumber, request.ManufacturingDate));
            return _createVehiclePresenter.ActionResult;
        }

        /// <summary>
        /// Retrieves all vehicles currently available for rental.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(
            typeof(ListVehiclesResponse),
            StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailableVehicles()
        {
            await _listVehiclesUseCase.Execute();

            var output = _listVehiclesPresenter.Response;

            var response = new ListVehiclesResponse(
                [.. output.Vehicles
                    .Select(vehicle => new VehicleResponse(
                        vehicle.Id,
                        vehicle.RegistrationNumber,
                        vehicle.ManufacturingDate,
                        vehicle.Status))]);

            return Ok(response);
        }
    }
}

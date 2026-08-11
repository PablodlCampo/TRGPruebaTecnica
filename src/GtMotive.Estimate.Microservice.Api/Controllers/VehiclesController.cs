using System;
using System.Linq;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Api.Models;
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
        private readonly ListVehiclesUseCase _listVehiclesUseCase;
        private readonly IOutputPortStandard<CreateVehicleOutput> _createVehicleOutputPort;
        private readonly IOutputPortStandard<ListVehiclesOutput> _listVehiclesOutputPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="VehiclesController"/> class.
        /// </summary>
        /// <param name="createVehicleUseCase">
        /// Use case responsible for creating vehicles.
        /// </param>
        /// <param name="listVehiclesUseCase">
        /// Use case responsible for retrieving available vehicles.
        /// </param>
        /// <param name="createVehicleOutputPort">
        /// Output port used to retrieve the vehicle creation result.
        /// </param>
        /// <param name="listVehiclesOutputPort">
        /// Output port used to retrieve the vehicle listing result.
        /// </param>
        public VehiclesController(
            IUseCase<CreateVehicleInput> createVehicleUseCase,
            ListVehiclesUseCase listVehiclesUseCase,
            IOutputPortStandard<CreateVehicleOutput> createVehicleOutputPort,
            IOutputPortStandard<ListVehiclesOutput> listVehiclesOutputPort)
        {
            ArgumentNullException.ThrowIfNull(createVehicleUseCase);
            ArgumentNullException.ThrowIfNull(listVehiclesUseCase);
            ArgumentNullException.ThrowIfNull(createVehicleOutputPort);
            ArgumentNullException.ThrowIfNull(listVehiclesOutputPort);

            _createVehicleUseCase = createVehicleUseCase;
            _listVehiclesUseCase = listVehiclesUseCase;
            _createVehicleOutputPort = createVehicleOutputPort;
            _listVehiclesOutputPort = listVehiclesOutputPort;
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

            await _createVehicleUseCase.Execute(
                new CreateVehicleInput(
                    request.RegistrationNumber,
                    request.ManufacturingDate.Value));

            var output = _createVehicleOutputPort.Response;

            var response = new CreateVehicleResponse(
                output.VehicleId,
                output.RegistrationNumber);

            return Created(
                new Uri($"/vehicles/{response.VehicleId}", UriKind.Relative),
                response);
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

            var output = _listVehiclesOutputPort.Response;

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

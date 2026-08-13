using System;
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
    /// Provides REST endpoints for managing vehicle rentals.
    /// </summary>
    [ApiController]
    [Route("rentals")]
    public sealed class RentalsController : ControllerBase
    {
        private readonly IUseCase<RentVehicleInput> _rentVehicleUseCase;
        private readonly IUseCase<ReturnVehicleInput> _returnVehicleUseCase;
        private readonly RentVehiclePresenter _rentVehiclePresenter;
        private readonly ReturnVehiclePresenter _returnVehiclePresenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="RentalsController"/> class.
        /// </summary>
        /// <param name="rentVehicleUseCase">
        /// Use case responsible for renting vehicles.
        /// </param>
        /// <param name="returnVehicleUseCase">
        /// Use case responsible for returning rented vehicles.
        /// </param>
        /// <param name="rentVehiclePresenter">
        /// Presenter used to format the vehicle rental response.
        /// </param>
        /// <param name="returnVehiclePresenter">
        /// Presenter used to format the vehicle return response.
        /// </param>
        public RentalsController(
            IUseCase<RentVehicleInput> rentVehicleUseCase,
            IUseCase<ReturnVehicleInput> returnVehicleUseCase,
            RentVehiclePresenter rentVehiclePresenter,
            ReturnVehiclePresenter returnVehiclePresenter)
        {
            ArgumentNullException.ThrowIfNull(rentVehicleUseCase);
            ArgumentNullException.ThrowIfNull(returnVehicleUseCase);
            ArgumentNullException.ThrowIfNull(rentVehiclePresenter);
            ArgumentNullException.ThrowIfNull(returnVehiclePresenter);

            _rentVehicleUseCase = rentVehicleUseCase;
            _returnVehicleUseCase = returnVehicleUseCase;
            _rentVehiclePresenter = rentVehiclePresenter;
            _returnVehiclePresenter = returnVehiclePresenter;
        }

        /// <summary>
        /// Rents an available vehicle to a customer.
        /// </summary>
        /// <param name="request">
        /// Request containing the vehicle and customer identifiers.
        /// </param>
        /// <returns>
        /// The HTTP response containing the created rental.
        /// </returns>
        [HttpPost]
        [ProducesResponseType(
            typeof(RentVehicleResponse),
            StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Rent(
            [FromBody] RentVehicleRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            await _rentVehicleUseCase.Execute(
                new RentVehicleInput(
                    request.VehicleId,
                    request.CustomerId));

            return _rentVehiclePresenter.ActionResult;
        }

        /// <summary>
        /// Returns a rented vehicle and closes its active rental.
        /// </summary>
        /// <param name="rentalId">
        /// The unique identifier of the rental to return.
        /// </param>
        /// <returns>
        /// The HTTP response containing the completed rental information.
        /// </returns>
        [HttpPost("{rentalId:guid}/returns")]
        [ProducesResponseType(
            typeof(ReturnVehicleResponse),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Return(
            Guid rentalId)
        {
            await _returnVehicleUseCase.Execute(
                new ReturnVehicleInput(rentalId));

            return _returnVehiclePresenter.ActionResult;
        }
    }
}

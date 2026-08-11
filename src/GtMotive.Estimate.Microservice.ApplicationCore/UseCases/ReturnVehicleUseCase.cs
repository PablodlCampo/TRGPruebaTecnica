using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Interfaces;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases
{
    /// <summary>
    /// Use case responsible for returning a rented vehicle.
    /// </summary>
    /// <remarks>
    /// The use case retrieves the rental and vehicle, closes the rental,
    /// marks the vehicle as available and persists both changes.
    /// </remarks>
    public sealed class ReturnVehicleUseCase(
        IRentalRepository rentalRepository,
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork,
        IClock clock,
        IOutputPortStandard<ReturnVehicleOutput> outputPort)
        : IUseCase<ReturnVehicleInput>
    {
        private readonly IRentalRepository _rentalRepository = rentalRepository;
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IClock _clock = clock;
        private readonly IOutputPortStandard<ReturnVehicleOutput> _outputPort = outputPort;

        /// <summary>
        /// Executes the vehicle return use case.
        /// </summary>
        /// <param name="input">
        /// Input containing the identifier of the rental to return.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous execution of the use case.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the input is null.
        /// </exception>
        /// <exception cref="DomainException">
        /// Thrown when the rental or vehicle does not exist, or when the
        /// rental cannot be returned.
        /// </exception>
        public async Task Execute(ReturnVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var rental = await _rentalRepository.GetByIdAsync(input.RentalId)
                ?? throw new DomainException("The rental doesn't exist");

            var vehicle = await _vehicleRepository.GetByIdAsync(rental.VehicleId)
                ?? throw new DomainException("The vehicle doesn't exist");

            var returnedAt = _clock.UtcNow;

            rental.Return(returnedAt);
            vehicle.Return();

            await _rentalRepository.UpdateAsync(rental);
            await _vehicleRepository.UpdateAsync(vehicle);
            await _unitOfWork.Save();

            _outputPort.StandardHandle(
                new ReturnVehicleOutput(
                    rental.Id,
                    vehicle.Id,
                    rental.CustomerId,
                    returnedAt));
        }
    }

    /// <summary>
    /// Represents the input data required to return a rented vehicle.
    /// </summary>
    /// <param name="RentalId">
    /// The unique identifier of the rental to return.
    /// </param>
    public record ReturnVehicleInput(Guid RentalId) : IUseCaseInput;

    /// <summary>
    /// Represents the output data returned after successfully returning a vehicle.
    /// </summary>
    /// <param name="RentalId">
    /// The unique identifier of the completed rental.
    /// </param>
    /// <param name="VehicleId">
    /// The unique identifier of the returned vehicle.
    /// </param>
    /// <param name="CustomerId">
    /// The unique identifier of the customer who returned the vehicle.
    /// </param>
    /// <param name="ReturnedAt">
    /// The date and time when the vehicle was returned.
    /// </param>
    public record ReturnVehicleOutput(
        Guid RentalId,
        Guid VehicleId,
        Guid CustomerId,
        DateTime ReturnedAt) : IUseCaseOutput;
}

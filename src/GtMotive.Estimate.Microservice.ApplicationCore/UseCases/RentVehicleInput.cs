using System;

using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases
{
    /// <summary>
    /// Represents the input data required to rent a vehicle.
    /// </summary>
    /// <param name="VehicleId">The unique identifier of the vehicle to rent.</param>
    /// <param name="CustomerId">The unique identifier of the customer renting the vehicle.</param>
    public record RentVehicleInput(Guid VehicleId, Guid CustomerId) : IUseCaseInput;

    /// <summary>
    /// Represents the output data returned after successfully renting a vehicle.
    /// </summary>
    /// <param name="RentalId">The unique identifier of the rental.</param>
    /// <param name="VehicleId">The unique identifier of the rented vehicle.</param>
    /// <param name="CustomerId">The unique identifier of the customer who rented the vehicle.</param>
    public record RentVehicleOutput(
        Guid RentalId,
        Guid VehicleId,
        Guid CustomerId) : IUseCaseOutput;

    /// <summary>
    /// Use case responsible for renting an available vehicle to a customer.
    /// </summary>
    /// <remarks>
    /// The use case ensures that a customer does not have more than one active rental
    /// and that the requested vehicle is available before creating the rental.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="RentVehicleUseCase"/> class.
    /// </remarks>
    /// <param name="vehicleRepository">
    /// Repository used to retrieve and update vehicles.
    /// </param>
    /// <param name="rentalRepository">
    /// Repository used to retrieve and create rentals.
    /// </param>
    /// <param name="unitOfWork">
    /// Unit of work used to commit changes to the database.
    /// </param>
    /// <param name="outputPort">
    /// Output port used to return the result of the use case.
    /// </param>
    public class RentVehicleUseCase(
        IVehicleRepository vehicleRepository,
        IRentalRepository rentalRepository,
        IUnitOfWork unitOfWork,
        IOutputPortStandard<RentVehicleOutput> outputPort) : IUseCase<RentVehicleInput>
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
        private readonly IRentalRepository _rentalRepository = rentalRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IOutputPortStandard<RentVehicleOutput> _outputPort = outputPort;

        /// <summary>
        /// Executes the vehicle rental use case.
        /// </summary>
        /// <param name="input">
        /// Input containing the vehicle and customer identifiers.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous execution of the use case.
        /// </returns>
        /// <exception cref="DomainException">
        /// Thrown when the customer already has an active rental or when the requested
        /// vehicle does not exist or is not available.
        /// </exception>
        public async Task Execute(RentVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            // A customer cannot have more than one active rental at the same time.
            var activeRental = await _rentalRepository.GetActiveRentalByCustomerIdAsync(input.CustomerId);
            if (activeRental != null)
            {
                throw new DomainException("El cliente ya tiene un vehículo alquilado actualmente.");
            }

            // Retrieve the requested vehicle.
            var vehicle = await _vehicleRepository.GetByIdAsync(input.VehicleId)
                ?? throw new DomainException("El vehículo no existe");

            vehicle.Rent();

            var rental = Rental.Create(
                Guid.NewGuid(),
                vehicle.Id,
                input.CustomerId,
                DateTime.UtcNow);

            await _vehicleRepository.UpdateAsync(vehicle);
            await _rentalRepository.AddAsync(rental);
            await _unitOfWork.Save();

            _outputPort.StandardHandle(
                new RentVehicleOutput(
                    rental.Id,
                    vehicle.Id,
                    rental.CustomerId));
        }
    }
}

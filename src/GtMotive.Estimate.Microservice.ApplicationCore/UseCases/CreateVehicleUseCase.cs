using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Interfaces;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases
{
    /// <summary>
    /// Use case responsible for creating a new vehicle.
    /// </summary>
    /// <remarks>
    /// The use case creates a valid vehicle through the domain entity,
    /// persists it using the vehicle repository and commits the changes
    /// through the unit of work.
    /// </remarks>
    public sealed class CreateVehicleUseCase(
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork,
        IClock clock,
        IOutputPortStandard<CreateVehicleOutput> outputPort)
        : IUseCase<CreateVehicleInput>
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IClock _clock = clock;
        private readonly IOutputPortStandard<CreateVehicleOutput> _outputPort = outputPort;

        /// <summary>
        /// Executes the vehicle creation use case.
        /// </summary>
        /// <param name="input">
        /// Input containing the vehicle registration number and manufacturing date.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous execution of the use case.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the input is null.
        /// </exception>
        /// <exception cref="DomainException">
        /// Thrown when the vehicle data violates a domain rule.
        /// </exception>
        public async Task Execute(CreateVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var vehicle = Vehicle.Create(
                Guid.NewGuid(),
                input.RegistrationNumber,
                input.ManufacturingDate,
                _clock.UtcNow);

            await _vehicleRepository.AddAsync(vehicle);
            await _unitOfWork.Save();

            _outputPort.StandardHandle(
                new CreateVehicleOutput(
                    vehicle.Id,
                    vehicle.RegistrationNumber));
        }
    }

    /// <summary>
    /// Represents the input data required to create a vehicle.
    /// </summary>
    /// <param name="RegistrationNumber">
    /// The registration number of the vehicle.
    /// </param>
    /// <param name="ManufacturingDate">
    /// The date when the vehicle was manufactured.
    /// </param>
    public record CreateVehicleInput(
        string RegistrationNumber,
        DateTime ManufacturingDate) : IUseCaseInput;

    /// <summary>
    /// Represents the output data returned after successfully creating a vehicle.
    /// </summary>
    /// <param name="VehicleId">
    /// The unique identifier of the created vehicle.
    /// </param>
    /// <param name="RegistrationNumber">
    /// The registration number of the created vehicle.
    /// </param>
    public record CreateVehicleOutput(
        Guid VehicleId,
        string RegistrationNumber) : IUseCaseOutput;
}

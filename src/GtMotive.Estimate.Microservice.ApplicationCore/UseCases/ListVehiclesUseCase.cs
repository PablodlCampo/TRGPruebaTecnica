using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Enums;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases
{
    /// <summary>
    /// Use case responsible for retrieving the vehicles registered in the system.
    /// </summary>
    /// <remarks>
    /// The use case retrieves the vehicles through the vehicle repository
    /// and returns them through the output port.
    /// </remarks>
    /// <param name="vehicleRepository">
    /// Repository used to retrieve vehicles.
    /// </param>
    /// <param name="outputPort">
    /// Output port used to return the result of the use case.
    /// </param>
    public class ListVehiclesUseCase(
        IVehicleRepository vehicleRepository,
        IOutputPortStandard<ListVehiclesOutput> outputPort)
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
        private readonly IOutputPortStandard<ListVehiclesOutput> _outputPort = outputPort;

        /// <summary>
        /// Executes the vehicle listing use case.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous execution of the use case.
        /// </returns>
        public async Task Execute()
        {
            var vehicles = await _vehicleRepository.GetAvailableVehiclesAsync();

            var output = new ListVehiclesOutput(
                [.. vehicles
                    .Select(vehicle => new VehicleOutput(
                        vehicle.Id,
                        vehicle.RegistrationNumber,
                        vehicle.ManufacturingDate,
                        vehicle.Status))]);

            _outputPort.StandardHandle(output);
        }
    }

    /// <summary>
    /// Represents the vehicle data returned by the application layer.
    /// </summary>
    /// <param name="Id">
    /// The unique identifier of the vehicle.
    /// </param>
    /// <param name="RegistrationNumber">
    /// The registration number of the vehicle.
    /// </param>
    /// <param name="ManufacturingDate">
    /// The date when the vehicle was manufactured.
    /// </param>
    /// <param name="Status">
    /// The current status of the vehicle.
    /// </param>
    public record VehicleOutput(
        Guid Id,
        string RegistrationNumber,
        DateTime ManufacturingDate,
        VehicleStatus Status);

    /// <summary>
    /// Represents the output returned by the list vehicles use case.
    /// </summary>
    /// <param name="Vehicles">
    /// The collection of vehicles returned by the use case.
    /// </param>
    public record ListVehiclesOutput(
        IReadOnlyCollection<VehicleOutput> Vehicles) : IUseCaseOutput;
}

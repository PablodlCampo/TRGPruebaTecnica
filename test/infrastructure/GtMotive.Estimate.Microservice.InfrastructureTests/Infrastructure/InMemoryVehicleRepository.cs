using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Fakes
{
    /// <summary>
    /// In-memory implementation of <see cref="IVehicleRepository"/> used to
    /// isolate infrastructure tests from a real MongoDB dependency.
    /// </summary>
    internal sealed class InMemoryVehicleRepository : IVehicleRepository
    {
        private readonly ConcurrentDictionary<Guid, Vehicle> _vehicles = new();

        /// <summary>
        /// Adds a vehicle to the in-memory store.
        /// </summary>
        /// <param name="vehicle">The vehicle to add.</param>
        /// <returns>A completed task.</returns>
        public Task AddAsync(Vehicle vehicle)
        {
            _vehicles[vehicle.Id] = vehicle;
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves a vehicle by its identifier from the in-memory store.
        /// </summary>
        /// <param name="id">The vehicle identifier.</param>
        /// <returns>The matching vehicle, or <c>null</c> if not found.</returns>
        public Task<Vehicle> GetByIdAsync(Guid id)
        {
            _vehicles.TryGetValue(id, out var vehicle);
            return Task.FromResult(vehicle);
        }

        public Task UpdateAsync(Vehicle vehicle)
        {
            throw new NotImplementedException();
        }

        // NOTA: añade aquí el resto de miembros que defina IVehicleRepository
        // en tu proyecto (p. ej. GetAllAsync, ExistsAsync...) para que compile.
    }
}

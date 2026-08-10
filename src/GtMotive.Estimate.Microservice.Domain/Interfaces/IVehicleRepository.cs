using System;
using System.Collections.Generic;

using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Entities;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces
{
    /// <summary>
    /// Defines the contract for persistence and retrieval operations related to vehicles.
    /// </summary>
    public interface IVehicleRepository
    {
        /// <summary>
        /// Adds a new vehicle to the repository.
        /// </summary>
        /// <param name="vehicle">The vehicle to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddAsync(Vehicle vehicle);

        /// <summary>
        /// Retrieves a vehicle by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the vehicle.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// the vehicle if found; otherwise, <see langword="null"/>.
        /// </returns>
        Task<Vehicle> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves all vehicles that are currently available for rental.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// the collection of available vehicles.
        /// </returns>
        Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync();

        /// <summary>
        /// Updates an existing vehicle in the repository.
        /// </summary>
        /// <param name="vehicle">The vehicle with the updated information.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateAsync(Vehicle vehicle);
    }
}

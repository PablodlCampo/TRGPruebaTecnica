using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Entities;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces
{
    /// <summary>
    /// Defines the contract for persistence and retrieval operations related to rentals.
    /// </summary>
    public interface IRentalRepository
    {
        /// <summary>
        /// Adds a new rental to the repository.
        /// </summary>
        /// <param name="rental">The rental to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddAsync(Rental rental);

        /// <summary>
        /// Retrieves the active rental associated with a customer.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// the active rental if one exists; otherwise, <see langword="null"/>.
        /// </returns>
        Task<Rental> GetActiveRentalByCustomerIdAsync(Guid customerId);

        /// <summary>
        /// Updates an existing rental in the repository.
        /// </summary>
        /// <param name="rental">The rental with the updated information.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateAsync(Rental rental);
    }
}

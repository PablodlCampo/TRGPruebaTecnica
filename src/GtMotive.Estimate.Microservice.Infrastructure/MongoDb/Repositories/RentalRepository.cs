using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Repositories
{
    /// <summary>
    /// MongoDB implementation of the rental repository.
    /// </summary>
    public sealed class RentalRepository : IRentalRepository
    {
        private readonly IMongoCollection<Rental> _rentals;

        /// <summary>
        /// Initializes a new instance of the <see cref="RentalRepository"/> class.
        /// </summary>
        /// <param name="mongoService">
        /// Service used to create the MongoDB client.
        /// </param>
        /// <param name="options">
        /// MongoDB configuration options.
        /// </param>
        public RentalRepository(
            MongoService mongoService,
            IOptions<MongoDbSettings> options)
        {
            ArgumentNullException.ThrowIfNull(mongoService);
            ArgumentNullException.ThrowIfNull(options);

            var database = mongoService.MongoClient.GetDatabase(
                options.Value.MongoDbDatabaseName);

            _rentals = database.GetCollection<Rental>("Rentals");
        }

        /// <summary>
        /// Adds a new rental to MongoDB.
        /// </summary>
        /// <param name="rental">The rental to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddAsync(Rental rental)
        {
            ArgumentNullException.ThrowIfNull(rental);

            await _rentals.InsertOneAsync(rental);
        }

        /// <summary>
        /// Retrieves a rental by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the rental.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result
        /// contains the rental if found; otherwise, <see langword="null"/>.
        /// </returns>
        public async Task<Rental> GetByIdAsync(Guid id)
        {
            return await _rentals
                .Find(rental => rental.Id == id)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves the active rental associated with a customer.
        /// </summary>
        /// <param name="customerId">
        /// The unique identifier of the customer.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result
        /// contains the active rental if one exists; otherwise,
        /// <see langword="null"/>.
        /// </returns>
        public async Task<Rental> GetActiveRentalByCustomerIdAsync(Guid customerId)
        {
            return await _rentals
                .Find(rental =>
                    rental.CustomerId == customerId &&
                    rental.ReturnedAt == null)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Updates an existing rental in MongoDB.
        /// </summary>
        /// <param name="rental">The rental with the updated information.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task UpdateAsync(Rental rental)
        {
            ArgumentNullException.ThrowIfNull(rental);

            await _rentals.ReplaceOneAsync(
                existingRental => existingRental.Id == rental.Id,
                rental);
        }
    }
}

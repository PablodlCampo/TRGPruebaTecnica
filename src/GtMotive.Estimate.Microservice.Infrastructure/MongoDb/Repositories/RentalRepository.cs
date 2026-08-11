using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Documents;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Repositories
{
    /// <summary>
    /// MongoDB implementation of the rental repository.
    /// </summary>
    public sealed class RentalRepository : IRentalRepository
    {
        private readonly IMongoCollection<RentalDocument> _rentals;

        /// <summary>
        /// Initializes a new instance of the <see cref="RentalRepository"/> class.
        /// </summary>
        /// <param name="mongoService">
        /// Service used to access the MongoDB database.
        /// </param>
        public RentalRepository(MongoService mongoService)
        {
            ArgumentNullException.ThrowIfNull(mongoService);

            _rentals = mongoService.Database
                .GetCollection<RentalDocument>("Rentals");
        }

        /// <summary>
        /// Adds a new rental to MongoDB.
        /// </summary>
        /// <param name="rental">The rental to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddAsync(Rental rental)
        {
            ArgumentNullException.ThrowIfNull(rental);

            var document = ToDocument(rental);

            await _rentals.InsertOneAsync(document);
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
            var document = await _rentals
                .Find(rental => rental.Id == id)
                .FirstOrDefaultAsync();

            return document == null
                ? null
                : ToDomain(document);
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
            var document = await _rentals
                .Find(rental =>
                    rental.CustomerId == customerId &&
                    rental.ReturnedAt == null)
                .FirstOrDefaultAsync();

            return document == null
                ? null
                : ToDomain(document);
        }

        /// <summary>
        /// Updates an existing rental in MongoDB.
        /// </summary>
        /// <param name="rental">The rental with the updated information.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task UpdateAsync(Rental rental)
        {
            ArgumentNullException.ThrowIfNull(rental);

            var document = ToDocument(rental);

            await _rentals.ReplaceOneAsync(
                existingRental => existingRental.Id == rental.Id,
                document);
        }

        /// <summary>
        /// Converts a domain rental into its MongoDB persistence representation.
        /// </summary>
        /// <param name="rental">The domain rental to convert.</param>
        /// <returns>A MongoDB document representing the rental.</returns>
        private static RentalDocument ToDocument(Rental rental)
        {
            return new RentalDocument
            {
                Id = rental.Id,
                VehicleId = rental.VehicleId,
                CustomerId = rental.CustomerId,
                RentedAt = rental.RentedAt,
                ReturnedAt = rental.ReturnedAt
            };
        }

        /// <summary>
        /// Converts a MongoDB rental document into its domain representation.
        /// </summary>
        /// <param name="document">The MongoDB document to convert.</param>
        /// <returns>A rental reconstructed from the persisted state.</returns>
        private static Rental ToDomain(RentalDocument document)
        {
            return Rental.Rehydrate(
                document.Id,
                document.VehicleId,
                document.CustomerId,
                document.RentedAt,
                document.ReturnedAt);
        }
    }
}

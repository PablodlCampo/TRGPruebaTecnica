using System;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Documents
{
    /// <summary>
    /// Represents the MongoDB persistence model for a rental.
    /// </summary>
    public sealed class RentalDocument
    {
        /// <summary>
        /// Gets or sets the unique identifier of the rental.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the rented vehicle.
        /// </summary>
        public Guid VehicleId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the customer who rented the vehicle.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the rental started.
        /// </summary>
        public DateTime RentedAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the vehicle was returned.
        /// </summary>
        public DateTime? ReturnedAt { get; set; }
    }
}

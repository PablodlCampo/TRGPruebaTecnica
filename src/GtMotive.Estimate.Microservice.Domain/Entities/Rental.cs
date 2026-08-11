using System;

namespace GtMotive.Estimate.Microservice.Domain.Entities
{
    /// <summary>
    /// Represents a vehicle rental.
    /// A rental belongs to a customer and a vehicle and remains active
    /// until the vehicle is returned.
    /// </summary>
    public sealed class Rental
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rental"/> class.
        /// </summary>
        /// <param name="id">The unique identifier of the rental.</param>
        /// <param name="vehicleId">The identifier of the rented vehicle.</param>
        /// <param name="customerId">The identifier of the customer.</param>
        /// <param name="rentedAt">The date and time when the rental started.</param>
        private Rental(
            Guid id,
            Guid vehicleId,
            Guid customerId,
            DateTime rentedAt)
        {
            Id = id;
            VehicleId = vehicleId;
            CustomerId = customerId;
            RentedAt = rentedAt;
        }

        /// <summary>
        /// Gets the unique identifier of the rental.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the identifier of the rented vehicle.
        /// </summary>
        public Guid VehicleId { get; private set; }

        /// <summary>
        /// Gets the identifier of the customer who rented the vehicle.
        /// </summary>
        public Guid CustomerId { get; private set; }

        /// <summary>
        /// Gets the date and time when the rental started.
        /// </summary>
        public DateTime RentedAt { get; private set; }

        /// <summary>
        /// Gets the date and time when the vehicle was returned.
        /// Null when the rental is still active.
        /// </summary>
        public DateTime? ReturnedAt { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the rental is currently active.
        /// </summary>
        public bool IsActive => !ReturnedAt.HasValue;

        /// <summary>
        /// Creates a new active vehicle rental.
        /// </summary>
        /// <param name="id">The unique identifier of the rental.</param>
        /// <param name="vehicleId">The identifier of the vehicle being rented.</param>
        /// <param name="customerId">The identifier of the customer.</param>
        /// <param name="rentedAt">The date and time when the rental starts.</param>
        /// <returns>A new active rental.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when any identifier is empty.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the rental date is invalid.
        /// </exception>
        public static Rental Create(
            Guid id,
            Guid vehicleId,
            Guid customerId,
            DateTime rentedAt)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException(
                    "Rental id cannot be empty.",
                    nameof(id));
            }

            if (customerId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Customer id cannot be empty.",
                    nameof(id));
            }

            if (vehicleId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Vehicle id cannot be empty.",
                    nameof(vehicleId));
            }

            return new Rental(
                id,
                vehicleId,
                customerId,
                rentedAt);
        }

        /// <summary>
        /// Returns the rented vehicle and closes the rental.
        /// </summary>
        /// <param name="returnedAt">
        /// The date and time when the vehicle is returned.
        /// </param>
        /// <exception cref="DomainException">
        /// Thrown when the rental has already been returned.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the return date is earlier than the rental date.
        /// </exception>
        public void Return(DateTime returnedAt)
        {
            if (ReturnedAt.HasValue)
            {
                throw new DomainException($"Rental {Id} has already been returned");
            }

            if (returnedAt < RentedAt)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(returnedAt),
                    "Return date cannot be earlier than the rental date.");
            }

            ReturnedAt = returnedAt;
        }
    }
}

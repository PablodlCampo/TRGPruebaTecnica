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
        /// <param name="returnedAt">The date and time when the rental was returned at.</param>
        private Rental(
            Guid id,
            Guid vehicleId,
            Guid customerId,
            DateTime rentedAt,
            DateTime? returnedAt)
        {
            Id = id;
            VehicleId = vehicleId;
            CustomerId = customerId;
            RentedAt = rentedAt;
            ReturnedAt = returnedAt;
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
        /// <exception cref="DomainException">
        /// Thrown when Rental id is empty.
        /// </exception>
        /// <exception cref="DomainException">
        /// Thrown when Customer id is empty.
        /// </exception>
        /// <exception cref="DomainException">
        /// Thrown when Vehicle id is empty.
        /// </exception>
        public static Rental Create(
            Guid id,
            Guid vehicleId,
            Guid customerId,
            DateTime rentedAt)
        {
            if (id == Guid.Empty)
            {
                throw new DomainException("Rental id cannot be empty.");
            }

            if (customerId == Guid.Empty)
            {
                throw new DomainException("Customer id cannot be empty.");
            }

            if (vehicleId == Guid.Empty)
            {
                throw new DomainException("Vehicle id cannot be empty.");
            }

            return new Rental(
                id,
                vehicleId,
                customerId,
                rentedAt,
                null);
        }

        /// <summary>
        /// Reconstructs a rental from its persisted state.
        /// </summary>
        /// <param name="id">The unique identifier of the rental.</param>
        /// <param name="vehicleId">The identifier of the rented vehicle.</param>
        /// <param name="customerId">The identifier of the customer.</param>
        /// <param name="rentedAt">The date and time when the rental started.</param>
        /// <param name="returnedAt">The date and time when the rental was returned at.</param>
        /// <returns>A rental reconstructed from its persisted state.</returns>
        /// <remarks>
        /// This method is intended for use by the persistence layer when
        /// reconstructing an existing rental. It restores the complete
        /// persisted state without executing the rules used when creating
        /// a new rental.
        /// </remarks>
        public static Rental Rehydrate(
            Guid id,
            Guid vehicleId,
            Guid customerId,
            DateTime rentedAt,
            DateTime? returnedAt)
        {
            return new Rental(
                id,
                vehicleId,
                customerId,
                rentedAt,
                returnedAt);
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
        /// <exception cref="DomainException">
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
                throw new DomainException("Return date cannot be earlier than the rental date.");
            }

            ReturnedAt = returnedAt;
        }
    }
}

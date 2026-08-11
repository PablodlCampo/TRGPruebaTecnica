using System;
using GtMotive.Estimate.Microservice.Domain.Enums;

namespace GtMotive.Estimate.Microservice.Domain.Entities
{
    /// <summary>
    /// Represents a vehicle in the domain model.
    /// </summary>
    public sealed class Vehicle
    {
        private const int MaximumAgeInYears = 5;

        private Vehicle(
            Guid id,
            string registrationNumber,
            DateTime manufacturingDate,
            VehicleStatus status)
        {
            Id = id;
            RegistrationNumber = registrationNumber;
            ManufacturingDate = manufacturingDate;
            Status = status;
        }

        /// <summary>
        /// Gets the unique identifier of the vehicle.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the registration number of the vehicle.
        /// </summary>
        public string RegistrationNumber { get; private set; }

        /// <summary>
        /// Gets the manufacturing date of the vehicle.
        /// </summary>
        public DateTime ManufacturingDate { get; private set; }

        /// <summary>
        /// Gets the current status of the vehicle (Available or Rented).
        /// </summary>
        public VehicleStatus Status { get; private set; }

        /// <summary>
        /// Creates a new instance of the Vehicle class with the specified parameters.
        /// </summary>
        /// <param name="id">The unique identifier of the vehicle.</param>
        /// <param name="registrationNumber">The registration number of the vehicle.</param>
        /// <param name="manufacturingDate">The manufacturing date of the vehicle.</param>
        /// <param name="currentDate">The current date.</param>
        /// <returns>The created Vehicle instance.</returns>
        /// <exception cref="ArgumentException">Thrown when id is Guid.Empty, registrationNumber is null/empty/whitespace, or manufacturingDate is in the future.</exception>
        /// <exception cref="DomainException">Thrown when the vehicle is older than the allowed maximum age.</exception>
        public static Vehicle Create(
            Guid id,
            string registrationNumber,
            DateTime manufacturingDate,
            DateTime currentDate)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException(
                    "Vehicle id cannot be empty.",
                    nameof(id));
            }

            if (string.IsNullOrWhiteSpace(registrationNumber))
            {
                throw new ArgumentException(
                    "Registration number is required.",
                    nameof(registrationNumber));
            }

            if (manufacturingDate > currentDate)
            {
                throw new ArgumentException(
                    "Manufacturing date cannot be in the future.",
                    nameof(manufacturingDate));
            }

            if (manufacturingDate < currentDate.AddYears(-MaximumAgeInYears))
            {
                throw new DomainException("Vehicle cannot be older than 5 years.");
            }

            return new Vehicle(
                id,
                registrationNumber,
                manufacturingDate,
                VehicleStatus.Available);
        }

        /// <summary>
        /// Reconstructs a vehicle from its persisted state.
        /// </summary>
        /// <param name="id">The unique identifier of the vehicle.</param>
        /// <param name="registrationNumber">The vehicle registration number.</param>
        /// <param name="manufacturingDate">The vehicle manufacturing date.</param>
        /// <param name="status">The vehicle status.</param>
        /// <returns>A vehicle reconstructed from its persisted state.</returns>
        /// <remarks>
        /// This method is intended for use by the persistence layer when
        /// reconstructing an existing vehicle. Domain creation rules are not
        /// re-evaluated because the state has already been persisted.
        /// </remarks>
        public static Vehicle Rehydrate(
            Guid id,
            string registrationNumber,
            DateTime manufacturingDate,
            VehicleStatus status)
        {
            return new Vehicle(
                id,
                registrationNumber,
                manufacturingDate,
                status);
        }

        /// <summary>
        /// Rents the vehicle if it is currently available. If the vehicle is not available, a DomainException is thrown.
        /// </summary>
        /// <exception cref="DomainException">Thrown when the vehicle is not available for rent (Status is not Available).</exception>
        public void Rent()
        {
            if (Status != VehicleStatus.Available)
            {
                throw new DomainException("Vehicle is not available for rent.");
            }

            Status = VehicleStatus.Rented;
        }

        /// <summary>
        /// Returns the vehicle to the available status if it is currently rented. If the vehicle is not rented, a DomainException is thrown.
        /// </summary>
        /// <exception cref="DomainException">Thrown when the vehicle is not currently rented (Status is not Rented).</exception>
        public void Return()
        {
            if (Status != VehicleStatus.Rented)
            {
                throw new DomainException("Vehicle is not currently rented.");
            }

            Status = VehicleStatus.Available;
        }
    }
}

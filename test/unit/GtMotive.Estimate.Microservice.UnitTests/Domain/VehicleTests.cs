using System;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Enums;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore
{
    /// <summary>
    /// Class containing unit tests for the Vehicle class in the domain model.
    /// </summary>
    public sealed class VehicleTests
    {
        private static readonly DateTime Now = DateTime.UtcNow;

        /// <summary>
        /// Tests the creation of a Vehicle instance with valid parameters.
        /// </summary>
        [Fact]
        public void CreateWithValidParametersReturnVehicle()
        {
            var id = Guid.NewGuid();
            var registration = "1234-ABC";
            var manufacturingDate = Now.AddYears(-1);

            var vehicle = Vehicle.Create(id, registration, manufacturingDate, Now);

            Assert.Equal(id, vehicle.Id);
            Assert.Equal(registration, vehicle.RegistrationNumber);
            Assert.Equal(manufacturingDate, vehicle.ManufacturingDate);
            Assert.Equal(VehicleStatus.Available, vehicle.Status);
        }

        /// <summary>
        /// Tests that creating a Vehicle instance with a future manufacturing date throws an ArgumentException.
        /// </summary>
        [Fact]
        public void CreateWithFutureManufacturingDateThrowsArgumentException()
        {
            var id = Guid.NewGuid();
            var registration = "1234-ABC";
            var future = Now.AddDays(1);

            Assert.Throws<DomainException>(() =>
                Vehicle.Create(id, registration, future, Now));
        }

        /// <summary>
        /// Tests that creating a Vehicle instance with an empty GUID throws an ArgumentException.
        /// </summary>
        [Fact]
        public void CreateOlderThanMaximumAgeThrowsDomainException()
        {
            var id = Guid.NewGuid();
            var registration = "1234-ABC";

            var tooOld = Now.AddYears(-5).AddDays(-2);

            Assert.ThrowsAny<Exception>(() =>
                Vehicle.Create(id, registration, tooOld, Now));
        }

        /// <summary>
        /// Test to ensure that creating a Vehicle instance that is exactly five years old does not throw an exception.
        /// </summary>
        [Fact]
        public void CreateExactlyFiveYearsOld()
        {
            var id = Guid.NewGuid();
            var registration = "1234-ABC";

            var tooOld = Now.AddYears(-5);
            var vehicle = Vehicle.Create(id, registration, tooOld, Now);

            Assert.Equal(id, vehicle.Id);
        }

        /// <summary>
        /// Tests that renting a vehicle changes its status to Rented.
        /// </summary>
        [Fact]
        public void RentWhenNotAvailableThrowsDomainException()
        {
            var id = Guid.NewGuid();
            var registration = "1234-ABC";
            var manufacturingDate = Now.AddYears(-1);
            var vehicle = Vehicle.Create(id, registration, manufacturingDate, Now);

            vehicle.Rent();

            Assert.ThrowsAny<Exception>(vehicle.Rent);
        }

        /// <summary>
        /// Tests that returning a rented vehicle changes its status back to Available.
        /// </summary>
        [Fact]
        public void ReturnWhenRentedChangesStatusToAvailable()
        {
            var id = Guid.NewGuid();
            var registration = "1234-ABC";
            var manufacturingDate = Now.AddYears(-1);
            var vehicle = Vehicle.Create(id, registration, manufacturingDate, Now);

            vehicle.Rent();
            vehicle.Return();

            Assert.Equal(VehicleStatus.Available, vehicle.Status);
        }

        /// <summary>
        /// Tests that returning a vehicle that is not currently rented throws a DomainException.
        /// </summary>
        [Fact]
        public void ReturnWhenNotRentedThrowsDomainException()
        {
            var id = Guid.NewGuid();
            var registration = "1234-ABC";
            var manufacturingDate = Now.AddYears(-1);
            var vehicle = Vehicle.Create(id, registration, manufacturingDate, Now);

            Assert.ThrowsAny<Exception>(vehicle.Return);
        }
    }
}

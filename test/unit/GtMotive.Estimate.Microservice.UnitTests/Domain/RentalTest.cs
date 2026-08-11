using System;
using GtMotive.Estimate.Microservice.Domain.Entities;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore
{
    /// <summary>
    /// Class containing unit tests for the Rental class in the domain model.
    /// </summary>
    public sealed class RentalTest
    {
        private static readonly DateTime Now = DateTime.UtcNow;

        /// <summary>
        /// Tests that returning a rental sets the ReturnedAt property and makes the rental inactive.
        /// </summary>
        [Fact]
        public void ReturnWhenActiveSetsReturnedAtAndBecomesInactive()
        {
            var id = Guid.NewGuid();
            var vehicleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var rentedAt = Now;
            var returnedAt = Now.AddHours(2);

            var rental = Rental.Create(id, vehicleId, customerId, rentedAt);

            rental.Return(returnedAt);

            Assert.False(rental.IsActive);
            Assert.Equal(returnedAt, rental.ReturnedAt);
        }

        /// <summary>
        /// Tests that attempting to return a rental that has already been returned throws a DomainException.
        /// </summary>
        [Fact]
        public void ReturnWhenAlreadyReturnedThrowsDomainException()
        {
            var id = Guid.NewGuid();
            var vehicleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var rentedAt = Now;
            var returnedAt = Now.AddHours(2);

            var rental = Rental.Create(id, vehicleId, customerId, rentedAt);
            rental.Return(returnedAt);

            Assert.ThrowsAny<Exception>(() => rental.Return(returnedAt.AddHours(1)));
        }

        /// <summary>
        /// Tests that attempting to return a rental that has already been returned throws a DomainException.
        /// </summary>
        [Fact]
        public void ReturnWithReturnedAtEarlierThanRentedAtThrowsArgumentOutOfRangeException()
        {
            var id = Guid.NewGuid();
            var vehicleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var rentedAt = Now;
            var returnedAt = Now.AddHours(-1);

            var rental = Rental.Create(id, vehicleId, customerId, rentedAt);

            Assert.Throws<ArgumentOutOfRangeException>(() => rental.Return(returnedAt));
        }
    }
}

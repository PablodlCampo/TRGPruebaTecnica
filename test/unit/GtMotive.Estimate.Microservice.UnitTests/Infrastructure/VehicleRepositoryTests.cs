using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Enums;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Documents;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Repositories;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Xunit;

namespace GtMotive.Estimate.Microservice.InfrastructureTests.MongoDb.Repositories
{
    /// <summary>
    /// Contains integration tests for the <see cref="VehicleRepository"/> class.
    /// </summary>
    public sealed class VehicleRepositoryTests
    {
        private const string DatabaseName = "estimate-infrastructure-tests";

        /// <summary>
        /// Verifies that a vehicle can be persisted in MongoDB and subsequently
        /// retrieved using its identifier.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous test operation.
        /// </returns>
        [Fact]
        public async Task AddAsyncThenGetByIdAsyncReturnsPersistedVehicle()
        {
            // Arrange
            var settings = Options.Create(new MongoDbSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                MongoDbDatabaseName = DatabaseName
            });

            var mongoService = new MongoService(settings);
            var repository = new VehicleRepository(mongoService);

            var vehicleId = Guid.NewGuid();
            var manufacturingDate = DateTime.UtcNow.AddYears(-1);

            var vehicle = Vehicle.Create(
                vehicleId,
                "1234-ABC",
                manufacturingDate,
                DateTime.UtcNow);

            try
            {
                // Act
                await repository.AddAsync(vehicle);

                var result = await repository.GetByIdAsync(vehicleId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(vehicle.Id, result.Id);
                Assert.Equal(
                    vehicle.RegistrationNumber,
                    result.RegistrationNumber);
                Assert.Equal(
                vehicle.ManufacturingDate.ToUniversalTime().Ticks / TimeSpan.TicksPerMillisecond,
                result.ManufacturingDate.ToUniversalTime().Ticks / TimeSpan.TicksPerMillisecond);

                Assert.Equal(
                    VehicleStatus.Available,
                    result.Status);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new InvalidOperationException(
                    "An error occurred during the test execution.",
                    ex);
            }
            finally
            {
                // Cleanup
                var collection = mongoService.Database
                    .GetCollection<VehicleDocument>("Vehicles");

                var filter = Builders<VehicleDocument>.Filter.Eq(
                    document => document.Id,
                    vehicleId);

                await collection.DeleteOneAsync(filter);
            }
        }
    }
}

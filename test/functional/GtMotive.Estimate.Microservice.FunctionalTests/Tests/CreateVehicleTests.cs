using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GtMotive.Estimate.Microservice.FunctionalTests
{
    /// <summary>
    /// Contains functional tests for the vehicle creation use case.
    /// </summary>
    public sealed class CreateVehicleTests(
        CompositionRootTestFixture fixture)
        : FunctionalTestBase(fixture)
    {
        /// <summary>
        /// Verifies that creating a vehicle through the application layer
        /// persists the vehicle in MongoDB.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous test operation.
        /// </returns>
        [Fact]
        public async Task CreateVehicleWithValidDataPersistsVehicle()
        {
            var registrationNumber = $"TEST-{Guid.NewGuid():N}";
            var manufacturingDate = DateTime.UtcNow.AddYears(-1);

            await Fixture.UsingScope(async services =>
            {
                var useCase =
                    services.GetRequiredService<IUseCase<CreateVehicleInput>>();

                var outputPort =
                    services.GetRequiredService<
                        IOutputPortStandard<CreateVehicleOutput>>();

                var repository =
                    services.GetRequiredService<IVehicleRepository>();

                await useCase.Execute(
                    new CreateVehicleInput(
                        registrationNumber,
                        manufacturingDate));

                var output = outputPort.Response;

                var vehicle = await repository.GetByIdAsync(
                    output.VehicleId);

                Assert.NotNull(vehicle);
                Assert.Equal(output.VehicleId, vehicle.Id);
                Assert.Equal(
                    registrationNumber,
                    vehicle.RegistrationNumber);
                Assert.Equal(
                    manufacturingDate,
                    vehicle.ManufacturingDate);
            });
        }
    }
}

using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Enums;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore
{
    /// <summary>
    /// Unit tests for the RentVehicle use case.
    /// </summary>
    public sealed class RentVehicleUseCaseTests
    {
        private static readonly DateTime Now =
            new(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Verifies that renting a vehicle fails when the requested vehicle is not available.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteWhenCustomerHasActiveRentalThrowsDomainException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var vehicleId = Guid.NewGuid();

            var rentalRepository = new Mock<IRentalRepository>();
            var vehicleRepository = new Mock<IVehicleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var outputPort = new Mock<IOutputPortStandard<RentVehicleOutput>>();

            var existingRental = Rental.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                customerId,
                Now);

            rentalRepository
                .Setup(repository =>
                    repository.GetActiveRentalByCustomerIdAsync(customerId))
                .Callback<Guid>(id => Console.WriteLine($"CustomerId recibido: {id}"))
                .ReturnsAsync(existingRental);

            var useCase = new RentVehicleUseCase(
                vehicleRepository.Object,
                rentalRepository.Object,
                unitOfWork.Object,
                outputPort.Object);

            var input = new RentVehicleInput(
                vehicleId,
                customerId);

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(
                () => useCase.Execute(input));

            vehicleRepository.Verify(
                repository => repository.GetByIdAsync(It.IsAny<Guid>()),
                Times.Never);

            outputPort.Verify(
                port => port.StandardHandle(It.IsAny<RentVehicleOutput>()),
                Times.Never);
        }

        /// <summary>
        /// Verifies that renting a vehicle fails when the requested vehicle is not available.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteWhenVehicleIsNotAvailableThrowsDomainException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var vehicleId = Guid.NewGuid();

            var rentalRepository = new Mock<IRentalRepository>();
            var vehicleRepository = new Mock<IVehicleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var outputPort = new Mock<IOutputPortStandard<RentVehicleOutput>>();

            rentalRepository
                .Setup(repository =>
                    repository.GetActiveRentalByCustomerIdAsync(customerId))
                .ReturnsAsync((Rental)null);

            var vehicle = Vehicle.Create(
                vehicleId,
                "1234-ABC",
                Now.AddYears(-1),
                Now);

            vehicle.Rent();

            vehicleRepository
                .Setup(repository =>
                    repository.GetByIdAsync(vehicleId))
                .ReturnsAsync(vehicle);

            var useCase = new RentVehicleUseCase(
                vehicleRepository.Object,
                rentalRepository.Object,
                unitOfWork.Object,
                outputPort.Object);

            var input = new RentVehicleInput(
                customerId,
                vehicleId);

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(
                () => useCase.Execute(input));

            rentalRepository.Verify(
                repository => repository.AddAsync(It.IsAny<Rental>()),
                Times.Never);

            outputPort.Verify(
                port => port.StandardHandle(It.IsAny<RentVehicleOutput>()),
                Times.Never);
        }

        /// <summary>
        /// Verifies that a valid customer can rent an available vehicle successfully.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteWhenCustomerAndVehicleAreValidRentsVehicle()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var vehicleId = Guid.NewGuid();

            var rentalRepository = new Mock<IRentalRepository>();
            var vehicleRepository = new Mock<IVehicleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var outputPort = new Mock<IOutputPortStandard<RentVehicleOutput>>();

            rentalRepository
                .Setup(repository =>
                    repository.GetActiveRentalByCustomerIdAsync(customerId))
                .ReturnsAsync((Rental)null);

            var vehicle = Vehicle.Create(
                vehicleId,
                "1234-ABC",
                Now.AddYears(-1),
                Now);

            vehicleRepository
                .Setup(repository =>
                    repository.GetByIdAsync(vehicleId))
                .ReturnsAsync(vehicle);

            var useCase = new RentVehicleUseCase(
                vehicleRepository.Object,
                rentalRepository.Object,
                unitOfWork.Object,
                outputPort.Object);

            var input = new RentVehicleInput(
                customerId,
                vehicleId);

            // Act
            await useCase.Execute(input);

            // Assert
            Assert.Equal(VehicleStatus.Rented, vehicle.Status);

            vehicleRepository.Verify(
                repository => repository.UpdateAsync(vehicle),
                Times.Once);

            rentalRepository.Verify(
                repository => repository.AddAsync(
                    It.Is<Rental>(rental =>
                        rental.VehicleId == vehicleId &&
                        rental.CustomerId == customerId &&
                        rental.IsActive)),
                Times.Once);

            outputPort.Verify(
                port => port.StandardHandle(
                    It.Is<RentVehicleOutput>(output =>
                        output.VehicleId == vehicleId &&
                        output.CustomerId == customerId)),
                Times.Once);
        }

        /// <summary>
        /// Verifies that renting a vehicle fails when the requested vehicle does not exist.
        /// </summary>
        /// <returns>A task representing the asynchronous test operation.</returns>
        [Fact]
        public async Task ExecuteWhenVehicleDoesNotExistThrowsDomainException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var vehicleId = Guid.NewGuid();

            var rentalRepository = new Mock<IRentalRepository>();
            var vehicleRepository = new Mock<IVehicleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var outputPort = new Mock<IOutputPortStandard<RentVehicleOutput>>();

            rentalRepository
                .Setup(repository =>
                    repository.GetActiveRentalByCustomerIdAsync(customerId))
                .ReturnsAsync((Rental)null);

            vehicleRepository
                .Setup(repository =>
                    repository.GetByIdAsync(vehicleId))
                .ReturnsAsync((Vehicle)null);

            var useCase = new RentVehicleUseCase(
                vehicleRepository.Object,
                rentalRepository.Object,
                unitOfWork.Object,
                outputPort.Object);

            var input = new RentVehicleInput(
                customerId,
                vehicleId);

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(
                () => useCase.Execute(input));

            rentalRepository.Verify(
                repository => repository.AddAsync(It.IsAny<Rental>()),
                Times.Never);

            outputPort.Verify(
                port => port.StandardHandle(It.IsAny<RentVehicleOutput>()),
                Times.Never);
        }
    }
}

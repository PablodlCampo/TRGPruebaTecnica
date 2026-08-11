using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Interfaces;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Enums;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore
{
    /// <summary>
    /// Unit tests for the ReturnVehicle use case.
    /// </summary>
    public sealed class ReturnVehicleUseCaseTests
    {
        private static readonly DateTime Now =
            new(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Verifies that returning a non-existing rental throws a domain exception.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous execution of the use case.
        /// </returns>
        [Fact]
        public async Task ExecuteWhenRentalDoesNotExistThrowsDomainException()
        {
            // Arrange
            var rentalId = Guid.NewGuid();

            var rentalRepository = new Mock<IRentalRepository>();
            var vehicleRepository = new Mock<IVehicleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var clock = new Mock<IClock>();
            var outputPort = new Mock<IOutputPortStandard<ReturnVehicleOutput>>();

            rentalRepository
                .Setup(repository => repository.GetByIdAsync(rentalId))
                .ReturnsAsync((Rental)null);

            var useCase = new ReturnVehicleUseCase(
                rentalRepository.Object,
                vehicleRepository.Object,
                unitOfWork.Object,
                clock.Object,
                outputPort.Object);

            var input = new ReturnVehicleInput(rentalId);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<DomainException>(
                () => useCase.Execute(input));

            Assert.Equal("The rental doesn't exist", exception.Message);

            vehicleRepository.Verify(
                repository => repository.GetByIdAsync(It.IsAny<Guid>()),
                Times.Never);

            outputPort.Verify(
                port => port.StandardHandle(It.IsAny<ReturnVehicleOutput>()),
                Times.Never);
        }

        /// <summary>
        /// Verifies that returning a rental fails when its vehicle is not rented.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous execution of the use case.
        /// </returns>
        [Fact]
        public async Task ExecuteWhenVehicleIsNotRentedThrowsDomainException()
        {
            // Arrange
            var rentalId = Guid.NewGuid();
            var vehicleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var rentalRepository = new Mock<IRentalRepository>();
            var vehicleRepository = new Mock<IVehicleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var outputPort = new Mock<IOutputPortStandard<ReturnVehicleOutput>>();
            var clock = new Mock<IClock>();

            clock
                .Setup(currentClock => currentClock.UtcNow)
                .Returns(Now);

            var rental = Rental.Create(
                rentalId,
                vehicleId,
                customerId,
                Now);

            rentalRepository
                .Setup(repository =>
                    repository.GetByIdAsync(rentalId))
                .ReturnsAsync(rental);

            var vehicle = Vehicle.Create(
                vehicleId,
                "1234-ABC",
                Now.AddYears(-1),
                Now);

            // Vehicle remains Available instead of Rented.
            vehicleRepository
                .Setup(repository =>
                    repository.GetByIdAsync(vehicleId))
                .ReturnsAsync(vehicle);

            var useCase = new ReturnVehicleUseCase(
                rentalRepository.Object,
                vehicleRepository.Object,
                unitOfWork.Object,
                clock.Object,
                outputPort.Object);

            var input = new ReturnVehicleInput(rentalId);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<DomainException>(
                () => useCase.Execute(input));

            Assert.Equal(
                "Vehicle is not currently rented.",
                exception.Message);

            vehicleRepository.Verify(
                repository => repository.UpdateAsync(
                    It.IsAny<Vehicle>()),
                Times.Never);

            outputPort.Verify(
                port => port.StandardHandle(
                    It.IsAny<ReturnVehicleOutput>()),
                Times.Never);
        }

        /// <summary>
        /// Verifies that returning an already completed rental throws a domain exception.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous execution of the use case.
        /// </returns>
        [Fact]
        public async Task ExecuteWhenRentalIsAlreadyReturnedThrowsDomainException()
        {
            // Arrange
            var rentalId = Guid.NewGuid();
            var vehicleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var rentalRepository = new Mock<IRentalRepository>();
            var vehicleRepository = new Mock<IVehicleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var clock = new Mock<IClock>();
            var outputPort = new Mock<IOutputPortStandard<ReturnVehicleOutput>>();

            var rental = Rental.Create(
                rentalId,
                vehicleId,
                customerId,
                Now.AddHours(-2));

            rental.Return(Now.AddHours(-1));

            var vehicle = Vehicle.Create(
                vehicleId,
                "1234-ABC",
                Now.AddYears(-1),
                Now);

            vehicle.Rent();

            rentalRepository
                .Setup(repository => repository.GetByIdAsync(rentalId))
                .ReturnsAsync(rental);

            vehicleRepository
                .Setup(repository => repository.GetByIdAsync(vehicleId))
                .ReturnsAsync(vehicle);

            clock
                .Setup(currentClock => currentClock.UtcNow)
                .Returns(Now);

            var useCase = new ReturnVehicleUseCase(
                rentalRepository.Object,
                vehicleRepository.Object,
                unitOfWork.Object,
                clock.Object,
                outputPort.Object);

            var input = new ReturnVehicleInput(rentalId);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<DomainException>(
                () => useCase.Execute(input));

            Assert.Equal(
                $"Rental {rentalId} has already been returned",
                exception.Message);

            rentalRepository.Verify(
                repository => repository.UpdateAsync(It.IsAny<Rental>()),
                Times.Never);

            vehicleRepository.Verify(
                repository => repository.UpdateAsync(It.IsAny<Vehicle>()),
                Times.Never);

            outputPort.Verify(
                port => port.StandardHandle(It.IsAny<ReturnVehicleOutput>()),
                Times.Never);
        }

        /// <summary>
        /// Verifies that a valid rental is closed and its vehicle becomes available.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous execution of the use case.
        /// </returns>
        [Fact]
        public async Task ExecuteWhenRentalAndVehicleAreValidReturnsVehicle()
        {
            // Arrange
            var rentalId = Guid.NewGuid();
            var vehicleId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var rentalRepository = new Mock<IRentalRepository>();
            var vehicleRepository = new Mock<IVehicleRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var clock = new Mock<IClock>();
            var outputPort = new Mock<IOutputPortStandard<ReturnVehicleOutput>>();

            var rental = Rental.Create(
                rentalId,
                vehicleId,
                customerId,
                Now.AddHours(-2));

            var vehicle = Vehicle.Create(
                vehicleId,
                "1234-ABC",
                Now.AddYears(-1),
                Now);

            vehicle.Rent();

            rentalRepository
                .Setup(repository => repository.GetByIdAsync(rentalId))
                .ReturnsAsync(rental);

            vehicleRepository
                .Setup(repository => repository.GetByIdAsync(vehicleId))
                .ReturnsAsync(vehicle);

            clock
                .Setup(currentClock => currentClock.UtcNow)
                .Returns(Now);

            var useCase = new ReturnVehicleUseCase(
                rentalRepository.Object,
                vehicleRepository.Object,
                unitOfWork.Object,
                clock.Object,
                outputPort.Object);

            var input = new ReturnVehicleInput(rentalId);

            // Act
            await useCase.Execute(input);

            // Assert
            Assert.False(rental.IsActive);
            Assert.Equal(Now, rental.ReturnedAt);

            Assert.Equal(VehicleStatus.Available, vehicle.Status);

            rentalRepository.Verify(
                repository => repository.UpdateAsync(rental),
                Times.Once);

            vehicleRepository.Verify(
                repository => repository.UpdateAsync(vehicle),
                Times.Once);

            unitOfWork.Verify(
                work => work.Save(),
                Times.Once);

            outputPort.Verify(
                port => port.StandardHandle(
                    It.Is<ReturnVehicleOutput>(
                        output =>
                            output.RentalId == rentalId &&
                            output.VehicleId == vehicleId &&
                            output.CustomerId == customerId &&
                            output.ReturnedAt == Now)),
                Times.Once);
        }
    }
}

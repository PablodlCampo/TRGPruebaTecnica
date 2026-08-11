using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Entities;
using GtMotive.Estimate.Microservice.Domain.Enums;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Documents;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Repositories
{
    /// <summary>
    /// MongoDB implementation of the vehicle repository.
    /// </summary>
    public sealed class VehicleRepository : IVehicleRepository
    {
        private readonly IMongoCollection<VehicleDocument> _vehicles;

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleRepository"/> class.
        /// </summary>
        /// <param name="mongoService">
        /// Service used to access the MongoDB database.
        /// </param>
        public VehicleRepository(MongoService mongoService)
        {
            ArgumentNullException.ThrowIfNull(mongoService);

            _vehicles = mongoService.Database
                .GetCollection<VehicleDocument>("Vehicles");
        }

        /// <summary>
        /// Adds a new vehicle to MongoDB.
        /// </summary>
        /// <param name="vehicle">The vehicle to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddAsync(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            var document = ToDocument(vehicle);

            await _vehicles.InsertOneAsync(document);
        }

        /// <summary>
        /// Retrieves a vehicle by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the vehicle.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result
        /// contains the vehicle if found; otherwise, <see langword="null"/>.
        /// </returns>
        public async Task<Vehicle> GetByIdAsync(Guid id)
        {
            var document = await _vehicles
                .Find(vehicle => vehicle.Id == id)
                .FirstOrDefaultAsync();

            return document == null
                ? null
                : ToDomain(document);
        }

        /// <summary>
        /// Retrieves all vehicles that are currently available for rental.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result
        /// contains the collection of available vehicles.
        /// </returns>
        public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
        {
            var documents = await _vehicles
                .Find(vehicle => vehicle.Status == VehicleStatus.Available)
                .ToListAsync();

            return documents.Select(ToDomain);
        }

        /// <summary>
        /// Updates an existing vehicle in MongoDB.
        /// </summary>
        /// <param name="vehicle">The vehicle with the updated information.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task UpdateAsync(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            var document = ToDocument(vehicle);

            await _vehicles.ReplaceOneAsync(
                existingVehicle => existingVehicle.Id == vehicle.Id,
                document);
        }

        /// <summary>
        /// Converts a domain vehicle into its MongoDB persistence representation.
        /// </summary>
        /// <param name="vehicle">The domain vehicle to convert.</param>
        /// <returns>A MongoDB document representing the vehicle.</returns>
        private static VehicleDocument ToDocument(Vehicle vehicle)
        {
            return new VehicleDocument
            {
                Id = vehicle.Id,
                RegistrationNumber = vehicle.RegistrationNumber,
                ManufacturingDate = vehicle.ManufacturingDate,
                Status = vehicle.Status
            };
        }

        /// <summary>
        /// Converts a MongoDB vehicle document into its domain representation.
        /// </summary>
        /// <param name="document">The MongoDB document to convert.</param>
        /// <returns>A vehicle reconstructed from the persisted state.</returns>
        private static Vehicle ToDomain(VehicleDocument document)
        {
            return Vehicle.Rehydrate(
                document.Id,
                document.RegistrationNumber,
                document.ManufacturingDate,
                document.Status);
        }
    }
}

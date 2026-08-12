using System;
using System.Text.Json.Serialization;

namespace GtMotive.Estimate.Microservice.Api.Models
{
    /// <summary>
    /// Request used to rent a vehicle.
    /// </summary>
    public sealed class RentVehicleRequest
    {
        /// <summary>
        /// Gets or sets the identifier of the vehicle to rent.
        /// </summary>
        [JsonRequired]
        public Guid VehicleId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the customer renting the vehicle.
        /// </summary>
        [JsonRequired]
        public Guid CustomerId { get; set; }
    }
}

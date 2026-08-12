using System;

namespace GtMotive.Estimate.Microservice.Api.Models
{
    /// <summary>
    /// Represents the request received when creating a new vehicle.
    /// </summary>
    public sealed class CreateVehicleRequest
    {
        /// <summary>
        /// Gets or sets the vehicle registration number.
        /// </summary>
        public string RegistrationNumber { get; set; }

        /// <summary>
        /// Gets or sets the vehicle manufacturing date.
        /// </summary>
        public DateTime? ManufacturingDate { get; set; }
    }
}

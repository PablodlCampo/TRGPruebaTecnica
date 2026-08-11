using System;

namespace GtMotive.Estimate.Microservice.Api.Models
{
    /// <summary>
    /// Represents the response returned after creating a vehicle.
    /// </summary>
    /// <param name="VehicleId">
    /// The unique identifier of the created vehicle.
    /// </param>
    /// <param name="RegistrationNumber">
    /// The registration number of the created vehicle.
    /// </param>
    public sealed record CreateVehicleResponse(
        Guid VehicleId,
        string RegistrationNumber);
}

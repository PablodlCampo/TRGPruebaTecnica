using System;

namespace GtMotive.Estimate.Microservice.Api.Models
{
    /// <summary>
    /// Represents the response returned after returning a vehicle.
    /// </summary>
    /// <param name="VehicleId">
    /// The unique identifier of the returned vehicle.
    /// </param>
    /// <param name="ReturnedAt">
    /// The date and time when the vehicle was returned.
    /// </param>
    public sealed record ReturnVehicleResponse(
        Guid VehicleId,
        DateTime ReturnedAt);
}

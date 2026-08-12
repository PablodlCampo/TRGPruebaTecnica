using System;
using GtMotive.Estimate.Microservice.Domain.Enums;

namespace GtMotive.Estimate.Microservice.Api.Models
{
    /// <summary>
    /// Represents a vehicle returned by the API.
    /// </summary>
    /// <param name="Id">
    /// The unique identifier of the vehicle.
    /// </param>
    /// <param name="RegistrationNumber">
    /// The registration number of the vehicle.
    /// </param>
    /// <param name="ManufacturingDate">
    /// The date when the vehicle was manufactured.
    /// </param>
    /// <param name="Status">
    /// The current status of the vehicle.
    /// </param>
    public sealed record VehicleResponse(
        Guid Id,
        string RegistrationNumber,
        DateTime ManufacturingDate,
        VehicleStatus Status);
}

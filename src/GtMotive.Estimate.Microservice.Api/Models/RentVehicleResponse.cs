using System;

namespace GtMotive.Estimate.Microservice.Api.Models
{
    /// <summary>
    /// Represents the response returned after renting a vehicle.
    /// </summary>
    /// <param name="RentalId">
    /// The unique identifier of the rental transaction.
    /// </param>
    /// <param name="VehicleId">
    /// The unique identifier of the rented vehicle.
    /// </param>
    /// <param name="CustomerId">
    /// The unique identifier of the customer who rented the vehicle.
    /// </param>
    public sealed record RentVehicleResponse(
        Guid RentalId,
        Guid VehicleId,
        Guid CustomerId);
}

using System.Collections.Generic;

namespace GtMotive.Estimate.Microservice.Api.Models
{
    /// <summary>
    /// Represents the response returned when retrieving available vehicles.
    /// </summary>
    /// <param name="Vehicles">
    /// The collection of vehicles currently available for rental.
    /// </param>
    public sealed record ListVehiclesResponse(
        IReadOnlyCollection<VehicleResponse> Vehicles);
}

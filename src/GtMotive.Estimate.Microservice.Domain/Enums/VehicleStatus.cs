namespace GtMotive.Estimate.Microservice.Domain.Enums
{
    /// <summary>
    /// Represents the status of a vehicle, indicating whether it is available for rent or currently rented.
    /// </summary>
    public enum VehicleStatus
    {
        /// <summary>
        /// Indicates that the vehicle has no specific status (default value).
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates that the vehicle is available for rent.
        /// </summary>
        Available = 1,

        /// <summary>
        /// Indicates that the vehicle is currently rented and not available for rent.
        /// </summary>
        Rented = 2
    }
}

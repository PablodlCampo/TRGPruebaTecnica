using System;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.Infrastructure.TimeProviders
{
    /// <summary>
    /// Provides the current UTC date and time using the system clock.
    /// </summary>
    public sealed class SystemClock : IClock
    {
        /// <summary>
        /// Gets the current UTC date and time.
        /// </summary>
        public DateTime UtcNow => DateTime.UtcNow;
    }
}

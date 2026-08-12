using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb
{
    /// <summary>
    /// MongoDB implementation of the unit of work pattern.
    /// </summary>
    /// <remarks>
    /// MongoDB repository operations are executed immediately, therefore
    /// there is no pending transaction to commit in this implementation.
    /// The method is kept to satisfy the application abstraction defined
    /// by <see cref="IUnitOfWork"/>.
    /// </remarks>
    public sealed class MongoUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Applies all pending database changes.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The result is
        /// zero because MongoDB repository operations are executed immediately.
        /// </returns>
        public Task<int> Save()
        {
            return Task.FromResult(0);
        }
    }
}

using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Fakes
{
    /// <summary>
    /// No-op implementation of <see cref="IUnitOfWork"/> used in infrastructure
    /// tests, where there is no real persistence context to commit changes to.
    /// </summary>
    internal sealed class NoOpUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Simulates saving changes without any real persistence side effect.
        /// </summary>
        /// <returns>A task that resolves to zero affected rows.</returns>
        public Task<int> Save() => Task.FromResult(0);
    }
}

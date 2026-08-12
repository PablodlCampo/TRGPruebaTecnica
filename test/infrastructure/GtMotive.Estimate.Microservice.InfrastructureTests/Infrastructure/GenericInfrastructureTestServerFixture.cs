using System;
using System.IO;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.InfrastructureTests.Fakes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: CLSCompliant(false)]

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure
{
    /// <summary>
    /// Bootstraps an in-memory <see cref="TestServer"/> for infrastructure-level
    /// tests, exercising the full HTTP host pipeline (routing, model binding,
    /// filters, and controllers) while replacing external persistence
    /// dependencies with in-memory fakes.
    /// </summary>
    /// <remarks>
    /// Unlike functional tests, infrastructure tests are not meant to validate
    /// business logic or real persistence — only that the host correctly
    /// receives, validates, and routes HTTP requests. Substituting the
    /// repository and unit of work keeps these tests fast, deterministic, and
    /// free of any external dependency such as MongoDB or Docker.
    /// </remarks>
    public sealed class GenericInfrastructureTestServerFixture : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericInfrastructureTestServerFixture"/> class,
        /// building an in-memory <see cref="TestServer"/> configured for
        /// infrastructure tests.
        /// </summary>
        public GenericInfrastructureTestServerFixture()
        {
            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseEnvironment("IntegrationTest")
                .UseDefaultServiceProvider(options => { options.ValidateScopes = true; })
                .ConfigureAppConfiguration((context, builder) => { builder.AddEnvironmentVariables(); })
                .UseStartup<Startup>()
                .ConfigureTestServices(services =>
                {
                    services.AddScoped<IVehicleRepository, InMemoryVehicleRepository>();
                    services.AddScoped<IUnitOfWork, NoOpUnitOfWork>();
                });

            Server = new TestServer(hostBuilder);
        }

        /// <summary>
        /// Gets the in-memory test server used to issue HTTP requests against
        /// the application's host pipeline.
        /// </summary>
        public TestServer Server { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            Server?.Dispose();
        }
    }
}

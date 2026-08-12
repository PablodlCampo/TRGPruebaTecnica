using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Api.Models;
using GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace GtMotive.Estimate.Microservice.InfrastructureTests
{
    /// <summary>
    /// Contains infrastructure-level tests for the vehicle creation HTTP endpoint.
    /// </summary>
    /// <remarks>
    /// These tests exercise the host pipeline (routing, model binding, and
    /// model validation) through <see cref="TestServer"/>, using in-memory
    /// fakes for persistence. They do not verify business logic correctness
    /// or real persistence, which are covered by unit and functional tests
    /// respectively.
    /// </remarks>
    public sealed class CreateVehicleHostTests(
        GenericInfrastructureTestServerFixture fixture)
        : InfrastructureTestBase(fixture)
    {
        /// <summary>
        /// Verifies that a valid request to create a vehicle is accepted
        /// by the host and returns a <c>201 Created</c> response with the
        /// expected response body.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous test operation.
        /// </returns>
        [Fact]
        public async Task PostVehicleWithValidBodyReturnsCreated()
        {
            using var client = Fixture.Server.CreateClient();
            client.DefaultRequestHeaders.AcceptEncoding.Clear();
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("identity"));

            var request = new CreateVehicleRequest
            {
                RegistrationNumber = $"TEST-{Guid.NewGuid():N}",
                ManufacturingDate = DateTime.UtcNow.AddYears(-2),
            };

            var response = await client.PostAsJsonAsync("/vehicles", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<CreateVehicleResponse>();

            Assert.NotNull(body);
            Assert.NotEqual(Guid.Empty, body.VehicleId);
            Assert.Equal(request.RegistrationNumber, body.RegistrationNumber);
        }

        /// <summary>
        /// Verifies that the host rejects a request missing the required
        /// <see cref="CreateVehicleRequest.RegistrationNumber"/> field with
        /// a <c>400 Bad Request</c> response, without reaching the
        /// application layer.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous test operation.
        /// </returns>
        [Fact]
        public async Task PostVehicleWithMissingRegistrationNumberReturnsBadRequest()
        {
            using var client = Fixture.Server.CreateClient();

            var invalidPayload = new
            {
                ManufacturingDate = DateTime.UtcNow.AddYears(-1),
            };

            var response = await client.PostAsJsonAsync("/vehicles", invalidPayload);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Verifies that the host rejects a request missing the required
        /// <see cref="CreateVehicleRequest.ManufacturingDate"/> field with
        /// a <c>400 Bad Request</c> response, without reaching the
        /// application layer.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous test operation.
        /// </returns>
        [Fact]
        public async Task PostVehicleWithMissingManufacturingDateReturnsBadRequest()
        {
            using var client = Fixture.Server.CreateClient();

            var invalidPayload = new
            {
                RegistrationNumber = $"TEST-{Guid.NewGuid():N}",
            };

            var response = await client.PostAsJsonAsync("/vehicles", invalidPayload);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}

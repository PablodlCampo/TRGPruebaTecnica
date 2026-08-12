using GtMotive.Estimate.Microservice.Api.Presenters;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GtMotive.Estimate.Microservice.Api.DependencyInjection
{
    /// <summary>
    /// Service collection extensions for user interface components.
    /// </summary>
    public static class UserInterfaceExtensions
    {
        /// <summary>
        /// Adds presenters and their output ports to the service collection.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <returns>The modified instance.</returns>
        public static IServiceCollection AddPresenters(this IServiceCollection services)
        {
            // Create Vehicle
            services.AddScoped<CreateVehiclePresenter>();
            services.AddScoped<IOutputPortStandard<CreateVehicleOutput>>(
                provider => provider.GetRequiredService<CreateVehiclePresenter>());

            // List Vehicles
            services.AddScoped<ListVehiclesPresenter>();
            services.AddScoped<IOutputPortStandard<ListVehiclesOutput>>(
                provider => provider.GetRequiredService<ListVehiclesPresenter>());

            // Rent Vehicle
            services.AddScoped<RentVehiclePresenter>();
            services.AddScoped<IOutputPortStandard<RentVehicleOutput>>(
                provider => provider.GetRequiredService<RentVehiclePresenter>());

            // Return Vehicle
            services.AddScoped<ReturnVehiclePresenter>();
            services.AddScoped<IOutputPortStandard<ReturnVehicleOutput>>(
                provider => provider.GetRequiredService<ReturnVehiclePresenter>());

            return services;
        }
    }
}

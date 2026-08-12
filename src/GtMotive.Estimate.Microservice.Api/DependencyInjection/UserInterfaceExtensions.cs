using GtMotive.Estimate.Microservice.Api.Presenters;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GtMotive.Estimate.Microservice.Api.DependencyInjection
{
    public static class UserInterfaceExtensions
    {
        public static IServiceCollection AddPresenters(this IServiceCollection services)
        {
            services.AddScoped<CreateVehiclePresenter>();
            services.AddScoped<ListVehiclesPresenter>();

            services.AddScoped<IOutputPortStandard<CreateVehicleOutput>>(
                provider => provider.GetRequiredService<CreateVehiclePresenter>());

            services.AddScoped<IOutputPortStandard<ListVehiclesOutput>>(
                provider => provider.GetRequiredService<ListVehiclesPresenter>());

            return services;
        }
    }
}

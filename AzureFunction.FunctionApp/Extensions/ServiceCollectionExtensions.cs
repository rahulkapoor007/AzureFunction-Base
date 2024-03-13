using AzureFunction.Application.Base;
using AzureFunction.Application.Base.Interfaces;
using AzureFunction.Application.Features.Users.CreateUsers;
using AzureFunction.Application.Features.Users.GetAllUsers;
using AzureFunction.Application.Features.Users.GetUserById;
using AzureFunction.Domain.Entities.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace AzureFunction.FunctionApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        internal static IHostBuilder AddInfrastructure(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.ConfigureServices(hostContext.Configuration);
                services.AddApplicationInsightsTelemetryWorkerService();
            });
            hostBuilder.ConfigureFunctionsWorkerDefaults();

            return hostBuilder;
        }

        private static IHostBuilder ConfigureFunctionsWorkerDefaults(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureFunctionsWorkerDefaults(worker => worker.UseMiddleware());
            return hostBuilder;
        }
        private static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<CreateUsersHandler>();
            services.AddScoped<GetAllUsersHandler>();
            services.AddScoped<GetUserByIdHandler>();
            services.AddSingleton<ConfigWrapper>();
            services.AddScoped<ICallContext, CallContext>();

            return services;
        }
        
    }
}

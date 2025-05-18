using Application.Commands;
using Application.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationServicesRegistration
{
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
    {

        services.AddScoped<UserCommands>();
        services.AddScoped<UserQueries>();

        return services;
    }
}

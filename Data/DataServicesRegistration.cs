using Data.Options;
using Data.Repositories;
using Data.Repositories.Initialization;
using Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Data;

public static class DataServicesRegistration
{
    public static IServiceCollection ConfigureDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var dsBuilder = new NpgsqlDataSourceBuilder(connectionString);
        var dbDataSource = dsBuilder.Build();

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(dbDataSource);
            options.EnableSensitiveDataLogging();
        });

        services.Configure<UsersOptions>(configuration.GetSection(nameof(UsersOptions)));
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<InitDefaultUsersService>();
        services.AddHostedService<AppDbContextMigratorHostedService>();

        return services;
    }
}
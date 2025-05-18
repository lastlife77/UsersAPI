using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace Data.Repositories.Initialization;

public class AppDbContextMigratorHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public AppDbContextMigratorHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var usersService = scope.ServiceProvider.GetRequiredService<InitDefaultUsersService>();

        context.Database.SetCommandTimeout(240);
        await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);

        await usersService.InitUsersAsync();

        context.Database.OpenConnection();
        (context.Database.GetDbConnection() as NpgsqlConnection)?.ReloadTypes();
        context.Database.CloseConnection();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

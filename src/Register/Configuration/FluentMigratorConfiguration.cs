using System.Reflection;
using FluentMigrator.Runner;

namespace AhoyRegister.Configuration;

public static class FluentMigratorConfiguration
{
    public static void RegisterFluentMigrator(this IServiceCollection services, string connectionString)
    {
        services.AddFluentMigratorCore()
            .ConfigureRunner(
                runnerBuilder =>
                {
                    runnerBuilder
                        .AddPostgres()
                        .WithGlobalConnectionString(connectionString)
                        .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations()
                        .For.All();
                })
            .AddLogging(lb => lb.AddFluentMigratorConsole());
    }

    public static void UseFluentMigratorConfiguration(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var migrator = scope.ServiceProvider.GetService<IMigrationRunner>();
        migrator.MigrateUp();
    }
}
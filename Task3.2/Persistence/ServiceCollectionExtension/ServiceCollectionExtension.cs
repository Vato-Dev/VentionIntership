using Application.Abstractions;
using Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence.PersistenceOptions;
using Persistence.Repositories;

namespace Persistence.ServiceCollectionExtension
{
    public static class ServiceCollectionExtension
    {
        public static void AddPersistence(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
            services.AddScoped<IMembershipRepository,MembershipRepository>();
        }
        public static void ConfigurePersistenceOptions(this IServiceCollection services)
        {
            services.AddOptions<DatabaseOptions>()
                .Configure(options => {
                    options.Host = "DB_HOST".FromEnv() ?? "localhost";
                    var portStr = "DB_PORT".FromEnv() ?? "5432";
                    options.Port = int.TryParse(portStr, out int port) ? port : 5432;
                    options.Host = "DB_HOST".FromEnvRequired();
                    options.Database = "DB_NAME".FromEnvRequired();
                    options.Username = "DB_USER".FromEnvRequired();
                    options.Password = "DB_PASSWORD".FromEnvRequired();
                })
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }
        public async static Task RunMigrationsAndSeed(this IServiceProvider services, ILogger logger, int maxRetries = 3)
        {
            var db = services.GetRequiredService<AppDbContext>();

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    logger.LogInformation("Migrating database...");
                    var pending = await db.Database.GetPendingMigrationsAsync();
                    if (pending.Any())
                    {
                        logger.LogInformation("Applying {Count} pending migrations...", pending.Count());
                        await db.Database.MigrateAsync();
                    }
                    await DatabaseSeeder.SeedAsync(db);

                    logger.LogDebug("Database migrations completed.");
                    return;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Database not ready (attempt {Attempt}/{Max})", i, maxRetries);
                }
                await Task.Delay(TimeSpan.FromSeconds(10)); // for retry
            }
            throw new Exception("Could not connect to the database after multiple retries");
        }
    }
}

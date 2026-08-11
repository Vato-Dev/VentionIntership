using Application.Abstractions;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.PersistenceOptions;

namespace Persistence.ServiceCollectionExtension
{
    public static class ServiceCollectionExtension
    {
        public static void AddPersistence(this IServiceCollection services)
        {           
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        }
        public static void ConfigurePersistenceOptions(this IServiceCollection services)
        {
            services.AddOptions<DatabaseOptions>() 
                .Configure(options => options.ConnectionString = Environment.GetEnvironmentVariable(DatabaseOptions.EnvironmentKey) ?? string.Empty)
                .ValidateDataAnnotations()
                .ValidateOnStart(); 
        }
    }
}

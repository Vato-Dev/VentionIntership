using Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence.ServiceCollectionExtension
{
    public static class ServiceCollectionExtension
    {
        public static void AddPersistence(this IServiceCollection services)
        {           
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        }
    }
}

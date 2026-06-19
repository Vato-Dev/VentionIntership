using LibraryManager.Application.Interfaces;
using LibraryManger.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using LibraryManger.Persistence.Querries;

namespace LibraryManger.Persistence.ServiceCollectionExtension
{
    public static class ServiceCollectionExtension
    {
        public static void RegisterPersistence(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IBorrowingRepository, BorrowingRepository>();
            services.AddScoped<IBookQuerries, BookQuerries>();
        }
    }
}

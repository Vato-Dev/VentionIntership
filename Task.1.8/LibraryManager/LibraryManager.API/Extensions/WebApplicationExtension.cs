using LibraryManager.Application.ServiceCollectionExtension;
using LibraryManger.Persistence;
using LibraryManger.Persistence.ServiceCollectionExtension;
using Microsoft.EntityFrameworkCore;

namespace LibraryManager.API.Extensions
{
    public static class WebApplicationExtension
    {
        public static void RegisterDb(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));
        }

        public static void RegisterApplicationLayer(this WebApplicationBuilder builder)
        {
            builder.Services.RegisterApplicationServices();
        }
        public static void RegisterPersistenceLayer(this WebApplicationBuilder builder)
        {
            builder.Services.RegisterPersistence();
        }
    }
    
}

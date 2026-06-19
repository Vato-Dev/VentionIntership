using LibraryManager.Application.Interfaces;
using LibraryManager.Application.Interfaces.Books;
using LibraryManager.Application.Services;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManager.Application.ServiceCollectionExtension
{
    public static class ServiceCollectionExtension
    {

        public static void RegisterApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IBookBorrowService, BookBorrowService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<ICatalogueService, CatalogueService>();
            services.AddScoped<IReaderService, ReaderService>();
        }
    }
}

using Application.Abstractions;
using Domain.Extensions;
using Infrastructure.FileManagement;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;

namespace Infrastructure.ServiceCollectionExtension
{
    public static class ServiceCollectionExtension
    {
        
        public static IServiceCollection AddRedisConfiguration(this IServiceCollection services)
        =>
            services.AddStackExchangeRedisCache(x => {
                x.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
                {
                    EndPoints = { "REDIS_HOST".FromEnvRequired() },
                    AbortOnConnectFail = true, //maybe I should not crash app if redis is dead 
                    ConnectRetry = 3,
                };
            });

        public static IServiceCollection AddFileSizeConfiguration(this IServiceCollection services, long maxBytes = 104857600 )
        =>
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = maxBytes;
            });
        
        public static IServiceCollection AddFileUploadServices(this IServiceCollection services)
        {
   
            services.AddScoped<FileValidationHelper>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<FileUploadService>();
            return services;
        }
    }
}

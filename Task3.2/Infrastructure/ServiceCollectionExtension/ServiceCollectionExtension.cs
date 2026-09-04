using Application.Abstractions;
using Domain.Extensions;
using Infrastructure.FileManagement;
using Infrastructure.Services;
using MassTransit;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;
using RabbitMQ.Client;

namespace Infrastructure.ServiceCollectionExtension
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRedisConfiguration(this IServiceCollection services)
            =>
                services.AddStackExchangeRedisCache(x => {
                    x.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
                    {
                        EndPoints =
                        {
                            "REDIS_HOST".FromEnvRequired()
                        },
                        AbortOnConnectFail = true, //maybe I should not crash app if redis is dead
                        ConnectRetry = 3,
                    };
                });

        public static IServiceCollection AddFileSizeConfiguration(this IServiceCollection services, long maxBytes = 104857600)
            =>
                services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = maxBytes; });

        public static IServiceCollection AddFileUploadServices(this IServiceCollection services)
        {
            services.AddScoped<FileValidationHelper>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IFileUploadService, FileUploadService>();
            return services;
        }
        public static IServiceCollection AddJtwConfiguration(this IServiceCollection services)
        {
            services.Configure<JwtOptions>(options => {
                options.Issuer = "JWT_ISSUER".FromEnvRequired();
                options.Audience = "JWT_AUDIENCE".FromEnvRequired();
                options.SecretKey = "JWT_KEY".FromEnvRequired();
                options.ExpirationInMinutes = int.Parse("JWT_EXPIRATION".FromEnvRequired());
            });
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
        public static IServiceCollection AddPasswordHasher(this IServiceCollection services)
            => services.AddScoped<IPasswordHasher, PasswordHasher>();

        public static IServiceCollection AddMessageBus(this IServiceCollection services)
        {
            services.AddMassTransit(x => {
                x.AddConsumer<FileProcessingConsumer>(cfg => {
                    cfg.UseConcurrencyLimit(8);
                    cfg.UseRateLimit(20, TimeSpan.FromSeconds(1));
                });

                x.UsingRabbitMq((context, cfg) => {
                    var host = "RABBITMQ_HOST".FromEnvRequired();
                    var username = "RABBITMQ_USERNAME".FromEnvRequired();
                    var password = "RABBITMQ_PASSWORD".FromEnvRequired();

                    cfg.Host(host, "/", h => {
                        h.Username(username);
                        h.Password(password);
                    });

                    cfg.UseMessageRetry(r => {
                        r.Interval(3, TimeSpan.FromSeconds(5));
                        r.Ignore<ArgumentNullException>();
                        r.Ignore<ArgumentException>();
                    });

                    cfg.ReceiveEndpoint("file-processing", e => {
                        e.ConfigureConsumer<FileProcessingConsumer>(context);
                        e.DiscardFaultedMessages();
                        e.UseInMemoryOutbox(); // i can use outbox pattern tied with db (and i should use it obviously) , but to keep things simple
                    });
                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddHealthChecks() // in theory if i already i can pull it outa DI or take fabric from DI , but i'm not sure if it will affect lifetime (it will ig)
                .AddRabbitMQ(
                sp =>
                {
                    var factory = new ConnectionFactory
                    {
                        Uri = new Uri($"amqp://{"RABBITMQ_USERNAME".FromEnvRequired()}:{"RABBITMQ_PASSWORD".FromEnvRequired()}@{"RABBITMQ_HOST".FromEnvRequired()}:5672")
                    };
                    return factory.CreateConnectionAsync();
                },
                name: "rabbitmq");


            return services;
        }
    }
}

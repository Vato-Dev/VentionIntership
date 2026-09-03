using Api.Hubs;
using Application.Abstractions;
using Infrastructure.ServiceCollectionExtension;

namespace Api.WebAppBuilderExtensions
{
    public static class WebAppBuilderExtensions
    {
        public static void ConfigureProblemDetails(this WebApplicationBuilder builder)
        {
            builder.Services.AddProblemDetails(op => {
                op.CustomizeProblemDetails = context => {
                    context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
                };
            });
        }

        public static void AddInfrastructure(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IFileStatusNotifier, SignalRFileStatusNotifier>();
            builder.Services
                .AddPasswordHasher()
                .AddJtwConfiguration()
                .AddRedisConfiguration()
                .AddFileSizeConfiguration()
                .AddFileUploadServices()
                .AddMessageBus();
        }
    }
}

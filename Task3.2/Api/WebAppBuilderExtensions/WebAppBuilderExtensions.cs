using Api.GraphQl;
using Api.Hubs;
using Application.Abstractions;
using Infrastructure.ServiceCollectionExtension;
using Serilog;

namespace Api.WebAppBuilderExtensions
{
    public static class WebAppBuilderExtensions
    {
        public static void ConfigureProblemDetails(this WebApplicationBuilder builder)
        {
            builder.Services.AddProblemDetails(op => { op.CustomizeProblemDetails = context => { context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier; }; });
        }

        public static void AddInfrastructure(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IFileStatusNotifier, SignalRFileStatusNotifier>();
            builder.Services.AddScoped<IChatNotifier, SignalRChatNotifier>();
            builder.Services
                .AddPasswordHasher()
                .AddJtwConfiguration()
                .AddRedisConfiguration()
                .AddFileSizeConfiguration()
                .AddFileUploadServices()
                .AddMessageBus();
        }

        public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(builder.Configuration)
                .CreateLogger();

            builder.Host.UseSerilog();
            return builder;
        }

        public static WebApplicationBuilder AddGraphQl(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddGraphQLServer()
                .AddQueryType<Query>()
                .AddMutationType<Mutation>()
                .AddFiltering()
                .AddSorting()
                .AddType<GraphQl.Types.FileStatusType>();

            return builder;
        }
    }
}

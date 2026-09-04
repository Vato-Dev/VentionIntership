using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Api.ExceptionHandlers;
using Api.Filters;
using Api.Hubs;
using Api.Middlewares;
using Api.WebAppBuilderExtensions;
using Application.SericeCollectionExtension;
using Domain.Extensions;
//using Application.SericeCollectionExtension;
//using Application.Validators;
using dotenv.net;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Persistence;
using Persistence.PersistenceOptions;
using Persistence.ServiceCollectionExtension;

var builder = WebApplication.CreateBuilder(args);

//todo : remove all regions and refactor Code to get rid of them
#region EnvConfiguration
DotEnv.Fluent()
    .WithOverwriteExistingVars()
    .WithTrimValues()
    .WithProbeForEnv()
    .Load();

#endregion

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
}).AddJsonOptions(options => {
   // options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; //5.6 task
    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString;//5.5 bigint task // i don't have long used so i made for int , works the same way
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 2 * 1024 * 1024; // 2 Megabytes for handling large json payloads 
});
//builder.Services.AddFluentValidationAutoValidation(); //todo make an action filter 
builder.Services.AddPersistence();
builder.AddInfrastructure();
builder.Services.ConfigurePersistenceOptions();

builder.Services.AddApplication();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value; 
    
    options.UseNpgsql(dbOptions.BuildConnectionString());
});

builder.Services.AddMemoryCache();
builder.ConfigureProblemDetails();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
//builder.Services.AddValidatorsFromAssemblyContaining<UserCreateDtoValidator>();

builder.AddSerilog();

builder.Services.AddAuthentication("GatewayTrust")
    .AddScheme<GatewayTrustOptions, GatewayTrustHandler>("GatewayTrust", options =>
    {
        options.SharedSecret = "YARP_GATEWAY_KEY".FromEnvRequired();
    });

builder.Services.AddSignalR();
builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();
builder.AddGraphQl();

var app = builder.Build();
app.UseExceptionHandler();
//app.UseMiddleware<GatewayTrustMiddleware>(); //i think i found issue i'm deleting there header and after handler can't find it and returns 401

using (var scope = app.Services.CreateScope())
{
    var logger =  scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
     await scope.ServiceProvider.RunMigrationsAndSeed(logger);
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/api/health"); 
app.MapGraphQL();
app.MapHub<FileStatusHub>("/hubs/files");

app.MapControllers();
app.Run();

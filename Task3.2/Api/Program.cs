using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Filters;
using Api.Middlewares;
using Api.WebAppBuilderExtensions;
using Application.SericeCollectionExtension;
using Application.Validators;
using dotenv.net;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Persistence;
using Persistence.Extensions;
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
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
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
builder.Services.ConfigurePersistenceOptions();

builder.Services.AddApplication();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
    
    options.UseNpgsql(dbOptions.ConnectionString);
});

builder.Services.AddMemoryCache();
builder.ConfigureProblemDetails();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<UserCreateDtoValidator>();

var app = builder.Build();
app.UseExceptionHandler();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();
//todo: delete real creds from appsettings
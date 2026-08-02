using Api.Consumers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMassTransit((configuration) => {
    configuration.UsingRabbitMq((context, cfg) => {
        var conString = builder.Configuration.GetConnectionString("RabbitMq");
        cfg.Host(conString);
        cfg.ConfigureEndpoints(context);
        cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(2)));
    });
    configuration.AddConsumer<RawDataIngestionConsumer>();
    configuration.AddConsumer<DataValidationConsumer>();
    configuration.AddConsumer<OrderCreatedConsumer>();
} );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

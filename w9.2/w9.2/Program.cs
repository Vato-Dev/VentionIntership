using MassTransit;
using MassTransit.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using w9._2.WorkerMock;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("OrderWorker"))
            .AddSource(DiagnosticHeaders.DefaultListenerName) 
            .AddOtlpExporter(); 
    });

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderProcessedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("order-processing-queue", e =>
        {
            e.ConcurrentMessageLimit = 2;  //because of this constraint 
            e.ConfigureConsumer<OrderProcessedConsumer>(context);
        });
    });
});

var host = builder.Build();
var runTask = host.RunAsync();
//to not make things simpler i'll just send this messages at start
var bus = host.Services.GetRequiredService<IBus>();
/*for (int i = 0; i < 10; i++)
{
    await bus.Publish(new OrderProcessedEvent(Guid.NewGuid()), context =>
    {
        context.CorrelationId = Guid.NewGuid(); 
    });    Console.WriteLine($"Published message {i + 1}");
}*/

await runTask;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace _9._6;

public record OrderProcessedEvent(Guid OrderId);

public class OrderProcessedConsumer : IConsumer<OrderProcessedEvent>
{
    public async Task Consume(ConsumeContext<OrderProcessedEvent> context)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] staring  {context.Message.OrderId}");

        throw new Exception($"Simulated failure for order {context.Message.OrderId}");
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

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
                    e.UseTimeout(t => t.Timeout = TimeSpan.FromSeconds(5)); 
                    e.ConcurrentMessageLimit = 2;
                    e.UseMessageRetry(r =>
                    {
                        r.Interval(3, TimeSpan.FromSeconds(2));
                        r.Ignore<ArgumentException>();
                    });
                    e.UseCircuitBreaker(cb =>
                    {
                        cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                        cb.TripThreshold = 15;  
                        cb.ActiveThreshold = 10;     
                        cb.ResetInterval = TimeSpan.FromMinutes(5);
                    });
                    e.ConfigureConsumer<OrderProcessedConsumer>(context);
                });
            });
        });

        var host = builder.Build();
        var runTask = host.RunAsync();
        await Task.Delay(2000);

        var bus = host.Services.GetRequiredService<IBus>();

        Console.WriteLine("Publishing 5 messages that will fail...");
        for (int i = 0; i < 5; i++)
        {
            var id = Guid.NewGuid();
            await bus.Publish(new OrderProcessedEvent(id));
            Console.WriteLine($"Published: {id}");
            await Task.Delay(300);
        }

        Console.WriteLine();
        Console.WriteLine("Messages published.");
        Console.WriteLine("Watch RabbitMQ Management:");
        Console.WriteLine("  - order-processing-queue");
        Console.WriteLine("  - order-processing-queue_error  <-- failures go here");
        Console.WriteLine();
        Console.WriteLine("Press Ctrl+C to stop...");

        await runTask; // works in rabbit ui it shows 5 errored 
    }
}
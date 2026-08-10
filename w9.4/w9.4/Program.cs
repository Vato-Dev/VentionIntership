using System;
using System.Threading.Tasks;
using MassTransit;
using NBomber.Contracts;
using NBomber.CSharp;
using Response=NBomber.CSharp.Response;

namespace w9._4;

public record StressTestMessage
{
    public Guid Id { get; init; }
    public DateTime Timestamp { get; init; }
}

class Program
{
    static async Task Main(string[] args)
    {
        var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host("localhost", "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });
        });

        await bus.StartAsync();

        var scenario = Scenario.Create("rabbitmq_publish_stress_test", async context =>
            {
                try
                {
                    await bus.Publish(new StressTestMessage
                    {
                        Id = Guid.NewGuid(),
                        Timestamp = DateTime.UtcNow
                    });

                    return Response.Ok();
                }
                catch (Exception ex)
                {
                    return Response.Fail(ex);
                }
            })
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            .WithLoadSimulations(
            Simulation.Inject(rate: 100, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)));

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();

        await bus.StopAsync(); //in rabbit ui i see 0 , was searching for reason why , Ai told me it's because of different queues i published StressTestMessage but worker from task 9.2 is listening for order-processing-queue
    }
}

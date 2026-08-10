using MassTransit;
using NBomber.CSharp;
using Response=NBomber.CSharp.Response;

namespace w9._2.WorkerMock;//Masstransit understood it as different messages because of namespaces so i did that on purpose

public record OrderProcessedEvent(Guid OrderId);

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
        Console.WriteLine("MassTransit bus started");

        var scenario = Scenario.Create("order_worker_stress_test", async context =>
            {
                try
                {
                    await bus.Publish(new OrderProcessedEvent(Guid.NewGuid()), publishCtx =>
                    {
                        publishCtx.CorrelationId = Guid.NewGuid();
                    });

                    return Response.Ok();
                }
                catch (Exception ex)
                {
                    return Response.Fail(statusCode: "500", message: ex.Message);
                }
            })
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            .WithLoadSimulations(Simulation.Inject(rate: 80, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(40)));

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithReportFolder("reports")
            .Run();

        await bus.StopAsync();
        Console.WriteLine("Done");
    }
}

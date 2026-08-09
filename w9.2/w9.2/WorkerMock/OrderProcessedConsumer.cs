using MassTransit;

namespace w9._2.WorkerMock
{
    public record OrderProcessedEvent(Guid OrderId);

    public class OrderProcessedConsumer : IConsumer<OrderProcessedEvent>
    {
        public async Task Consume(ConsumeContext<OrderProcessedEvent> context)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] START  {context.Message.OrderId}");
            await Task.Delay(1500);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] FINISH {context.Message.OrderId}");
        }
    }
}

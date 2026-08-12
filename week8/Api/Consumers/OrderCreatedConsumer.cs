using System.Collections.Concurrent;
using Api.Contracts;
using MassTransit;

namespace Api.Consumers
{
    public sealed class OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger) : IConsumer<OrderCreated>
    {
        private static readonly ConcurrentDictionary<Guid, bool> ProcessedOrders = new();

        public async Task Consume(ConsumeContext<OrderCreated> context)
        {
            var message = context.Message;
            if (ProcessedOrders.ContainsKey(message.OrderId))
            {
                logger.LogWarning("Order {OrderId} has been already processed.", message.OrderId);
                return;
            }

            logger.LogInformation("Processing order: {OrderId} ({ItemName})", message.OrderId, message.ItemName);

            if (message.ItemName.Equals("error", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogError("Error , using retry policies");
                throw new InvalidOperationException("failure");
            }

            await Task.Delay(1000);

            ProcessedOrders.TryAdd(message.OrderId, true);

            logger.LogInformation("Order {OrderId} successfully processed", message.OrderId);
        }
    }

}

using Api.Contracts;
using MassTransit;

namespace Api.Consumers
{
    public class OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger) : IConsumer<OrderCreated>
    {
        public Task Consume(ConsumeContext<OrderCreated> context)
        {
            logger.LogInformation("order is accepted {id}, ordername {name}", context.Message.OrderId, context.Message.ItemName);
            
            return Task.CompletedTask;
        }
    }
}

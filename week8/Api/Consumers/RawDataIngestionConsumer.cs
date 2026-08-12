using Api.Contracts;
using MassTransit;

namespace Api.Consumers
{
    public class RawDataIngestionConsumer(ILogger<RawDataIngestionConsumer> logger) : IConsumer<RawDataIngested>
    {
        public async Task Consume(ConsumeContext<RawDataIngested> context)
        {
            var message = context.Message;
            logger.LogInformation("Pipeline Step 1: Ingesting raw data {IngestionId}", message.IngestionId);

            string cleanedData = message.RawPayload.Trim().ToUpper();
            bool isValid = !string.IsNullOrEmpty(cleanedData);

            await Task.Delay(500); //delay just to immitate 

            await context.Publish(new DataValidated(message.IngestionId, cleanedData, isValid));

            logger.LogInformation("Pipeline Step 1 Completed: Sent to validation for {IngestionId}", message.IngestionId);
        }
    }
}

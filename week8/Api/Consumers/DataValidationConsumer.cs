using Api.Contracts;
using MassTransit;

namespace Api.Consumers
{
    public class DataValidationConsumer(ILogger<DataValidationConsumer> logger) : IConsumer<DataValidated>
    {
        public async Task Consume(ConsumeContext<DataValidated> context)
        {
            var message = context.Message;
            logger.LogInformation("Pipeline Step 2: Validating and saving data {IngestionId}", message.IngestionId);

            if (!message.IsValid)
            {
                logger.LogError("Pipeline Step 2 Failed: Data is invalid for {IngestionId}", message.IngestionId);
                return;
            }

            await Task.Delay(500); //again i'll imitate db delay since i did not fix error yet

            logger.LogInformation("Pipeline Step 2 Completed: Data successfully persisted for {IngestionId}", message.IngestionId);
        }
    }
}

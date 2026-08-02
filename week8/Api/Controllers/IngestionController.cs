using Api.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class IngestionController(IPublishEndpoint publishEndpoint) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> StartIngestion([FromQuery] string payload)
        {
            var ingestionId = Guid.NewGuid();
            var message = new RawDataIngested(ingestionId, payload, DateTime.UtcNow);

            await publishEndpoint.Publish(message);

            return Ok(new
            {
                Message = "Ingestion workflow started", IngestionId = ingestionId
            });
        }
    }
}

using Api.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    public class OrdersController(IPublishEndpoint publishEndpoint, ILogger<OrdersController> logger) : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
        private readonly ILogger<OrdersController> _logger = logger;

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromQuery] string itemName)
        {
            var message = new OrderCreated(Guid.NewGuid(), itemName);

            _logger.LogInformation("sent in Rabbit..");

            await _publishEndpoint.Publish(message);

            return Ok(new { text = "order is sent", order = message });
        }    }
}

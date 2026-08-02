using Api.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class OrdersController(IPublishEndpoint publishEndpoint, ILogger<OrdersController> logger) : ControllerBase
    {
        
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromQuery]string itemName, decimal price)
        {
            var orderId = Guid.NewGuid();
            var message = new OrderCreated(orderId, itemName, price);

            await publishEndpoint.Publish(message);
            
            logger.LogInformation($"Order created: {orderId}");
            return Ok(new
            {
                Message = "Order sent for processing", OrderId = orderId
            });
        }
    }
}

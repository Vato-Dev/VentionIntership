using BackendService.Models;
using BackendService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController(ProductRepository repository) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCart()
        {
            if (!Request.Headers.TryGetValue("X-User-Id", out var userId))
                return BadRequest(new
                {
                    error = "Error : context aint set X-User-Id"
                });

            return Ok(repository.GetCart(userId!));
        }

        [HttpPost("items")]
        public IActionResult AddItem([FromBody] AddToCartRequest request)
        {
            if (!Request.Headers.TryGetValue("X-User-Id", out var userId))
                return BadRequest(new
                {
                    error = "Error : context aint set X-User-Id"
                });

            var success = repository.AddToCart(userId!, request.ProductId, request.Quantity);
            return success
                ? Ok(new
                {
                    message = "added though API!"
                })
                : BadRequest(new
                {
                    error = "no items or incorrect id"
                });
        }
    }
}

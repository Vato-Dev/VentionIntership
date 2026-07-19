using BackendService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendService.Controllers
{

    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductRepository _repository;
        public ProductsController(ProductRepository repository) => _repository = repository;

        [HttpGet]
        public IActionResult GetAll() => Ok(_repository.GetAllProducts());

        [HttpGet("{id:guid}")]
        public IActionResult GetById(Guid id)
        {
            var product = _repository.GetProductById(id);
            return product != null ? Ok(product) : NotFound(new { error = "Товар не найден" });
        }
    }

}

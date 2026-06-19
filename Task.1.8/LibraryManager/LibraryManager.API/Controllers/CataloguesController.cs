using LibraryManager.Application.Interfaces;
using LibraryManager.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CataloguesController(ICatalogueService catalogueService) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateCatalogueRequest request, CancellationToken ct)
        {
            var id = await catalogueService.CreateCatalogue(request.Name, request.ParentId, ct);
            return StatusCode(StatusCodes.Status201Created, id);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Catalogue), StatusCodes.Status200OK)]
        public async Task<ActionResult<Catalogue>> Get(int id, CancellationToken ct)
        {
            var catalogue = await catalogueService.GetCatalogue(id, ct);
            return Ok(catalogue);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCatalogueRequest request, CancellationToken ct)
        {
            await catalogueService.UpdateCatalogue(id, request.NewName, request.NewParentId, ct);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await catalogueService.DeleteCatalogue(id, ct);
            return NoContent();
        }
    }

    public sealed record CreateCatalogueRequest(string Name, int? ParentId);
    public sealed record UpdateCatalogueRequest(string NewName, int? NewParentId);
}

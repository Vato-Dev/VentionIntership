using Application.Abstractions;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrganizationsController(IOrganizationService organizationService) : ControllerBase
    {
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var org = await organizationService.GetOrganizationByIdAsync(id, cancellationToken);
            if (org == null) return NotFound();
            return Ok(org);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid? keySetId, 
            [FromQuery] int? page = 1, 
            [FromQuery] int? pageSize = 10, 
            CancellationToken cancellationToken = default)
        {
            var result = await organizationService.GetAllOrganizationsAsync(keySetId, page, pageSize, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrganizationCreateDto dto, CancellationToken cancellationToken)
        {
            var createdOrg = await organizationService.CreateOrganizationAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = createdOrg.Id }, createdOrg);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] OrganizationUpdateDto dto, CancellationToken cancellationToken)
        {
            await organizationService.UpdateOrganizationAsync(id, dto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await organizationService.DeleteOrganizationAsync(id, cancellationToken);
            return NoContent();
        }
    }
}

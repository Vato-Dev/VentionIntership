using Application.Abstractions;
using Application.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class OrganizationsController(IOrganizationService organizationService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(
            CancellationToken cancellationToken,
            [FromQuery] int? keySetId = null,
            [FromQuery] int? page = 1,
            [FromQuery] int? pageSize = 10)
        {
            var pagedOrganizations = await organizationService.GetAllOrganizationsAsync(cancellationToken, keySetId, page, pageSize);

            var response = new PagedResponse<OrganizationResponseDto>
            {
                Data = pagedOrganizations.Data.Select(o => new OrganizationResponseDto
                {
                    Id = o.Id, Name = o.Name, StreetAddress = o.StreetAddress
                }).ToList(),
                PageNumber = pagedOrganizations.PageNumber,
                PageSize = pagedOrganizations.PageSize,
                TotalItems = pagedOrganizations.TotalItems,
                TotalPages = pagedOrganizations.TotalPages,
                LastSeenId = pagedOrganizations.LastSeenId
            };

            return Ok(response);
        }
    }
}

//         [HttpGet("{id:int}")]
//         public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
//         {
//             var org = await organizationService.GetOrganizationByIdAsync(cancellationToken,id);
//             if (org == null) return NotFound($"Organization with ID {id} not found.");
//
//             var response = new OrganizationResponseDto
//             {
//                 Id = org.Id,
//                 Name = org.Name,
//                 StreetAddress = org.StreetAddress
//             };
//             return Ok(response);
//         }
//
//         [HttpPost]
//         public async Task<IActionResult> Create([FromBody] OrganizationCreateDto dto, CancellationToken cancellationToken)
//         {
//             var org = new Organization
//             {
//                 Name = dto.Name,
//                 StreetAddress = dto.StreetAddress
//             };
//
//             try
//             {
//                 await organizationService.CreateOrganizationAsync(org, cancellationToken);
//                 
//                 var response = new OrganizationResponseDto
//                 {
//                     Id = org.Id,
//                     Name = org.Name,
//                     StreetAddress = org.StreetAddress
//                 };
//
//                 return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, $"Internal server error: {ex.Message}");
//             }
//         }
//
//         [HttpPut("{id:int}")]
//         public async Task<IActionResult> Update(int id, [FromBody] OrganizationUpdateDto dto, CancellationToken cancellationToken)
//         {
//             if (id != dto.Id) return BadRequest("Mismatched Organization ID.");
//
//             var existingOrg = await organizationService.GetOrganizationByIdAsync(cancellationToken,id);
//             if (existingOrg == null) return NotFound($"Organization with ID {id} not found.");
//
//             existingOrg.Name = dto.Name;
//             existingOrg.StreetAddress = dto.StreetAddress;
//
//             try
//             {
//                 await organizationService.UpdateOrganizationAsync(existingOrg, cancellationToken);
//                 return NoContent();
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, $"Internal server error: {ex.Message}");
//             }
//         }
//
//         [HttpDelete("{id:int}")]
//         public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
//         {
//             try
//             {
//                 var org = await organizationService.GetOrganizationByIdAsync(cancellationToken,id);
//                 if (org == null) return NotFound($"Organization with ID {id} not found.");
//
//                 await organizationService.DeleteOrganizationAsync(id, cancellationToken);
//                 return NoContent();
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, $"Internal server error: {ex.Message}");
//             }
//         }
//     }
// }
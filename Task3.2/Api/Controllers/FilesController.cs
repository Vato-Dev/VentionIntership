using System.Security.Claims;
using Api.Filters;
using Application.Abstractions;
using Infrastructure.FileManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class FilesController(FileUploadService fileUploadService, IMembershipRepository membershipRepository) : ControllerBase
    {
        [HttpPost]
        [Consumes("multipart/form-data")]
        [DisableFormValueModelBinding]
        [RequestSizeLimit(100 * 1024 * 1024)]
        public async Task<IActionResult> Upload(CancellationToken ct)
        {
            var ownerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(ownerIdClaim, out var ownerId))
                return Unauthorized();
            
 
            var orgIdRaw = Request.Headers["x-org-id"].FirstOrDefault();
            if (!Guid.TryParse(orgIdRaw, out var organisationId))
                return BadRequest(new { error = "Missing or invalid organisation id" });
            
 

            if (!await membershipRepository.IsMemberAsync(ownerId, organisationId, ct)) //not sure about sanity of this 
                return Forbid();
 
            var (statusCode, file, error) = await fileUploadService.HandleUploadAsync(Request, ownerId, organisationId, ct);
 
            if (file != null)
                return StatusCode(statusCode, file);
            
 
            return StatusCode(statusCode, new { error });
        }
    }
}

using System.Security.Claims;
using Api.Filters;
using Application.Abstractions;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class FilesController(
        IFileUploadService fileUploadService, 
        IMembershipRepository membershipRepository) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetFilesAsync(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10, 
            CancellationToken ct = default)
        {
            var validation = await ValidateRequestAsync(ct);
            if (validation.ErrorResult != null) return validation.ErrorResult;

            var result = await fileUploadService.GetFilesByOrganizationAsync(validation.OrganisationId, page, pageSize, ct);
            return Ok(result);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [DisableFormValueModelBinding]
        [RequestSizeLimit(100 * 1024 * 1024)]
        public async Task<IActionResult> Upload(CancellationToken ct)
        {
            var validation = await ValidateRequestAsync(ct);
            if (validation.ErrorResult != null) return validation.ErrorResult;
 
            var (statusCode, file, error) = await fileUploadService.HandleUploadAsync(Request, validation.OwnerId, validation.OrganisationId, ct);
 
            if (file != null) return StatusCode(statusCode, file);
            return StatusCode(statusCode, new { error });
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteFileAsync(Guid id, CancellationToken ct)
        {
            var validation = await ValidateRequestAsync(ct);
            if (validation.ErrorResult != null) return validation.ErrorResult;

            var (statusCode, error) = await fileUploadService.DeleteFileAsync(id, validation.OrganisationId, ct);
            
            if (statusCode == 204) return NoContent();
            return StatusCode(statusCode, new { error });
        }

        [HttpPost("{id:guid}/process")] 
        public async Task<IActionResult> ReprocessFileAsync(Guid id, CancellationToken ct)
        {
            var validation = await ValidateRequestAsync(ct);
            if (validation.ErrorResult != null) return validation.ErrorResult;

            var (statusCode, message, error) = await fileUploadService.ReprocessFileAsync(id, validation.OrganisationId, ct);

            if (error != null) return StatusCode(statusCode, new { error });
            return Ok(new { message });
        }

        private async Task<(Guid OwnerId, Guid OrganisationId, IActionResult? ErrorResult)> ValidateRequestAsync(CancellationToken ct)
        {
            var ownerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(ownerIdClaim, out var ownerId))
                return (Guid.Empty, Guid.Empty, Unauthorized());

            var orgIdRaw = Request.Headers["x-org-id"].FirstOrDefault();
            if (!Guid.TryParse(orgIdRaw, out var organisationId))
                return (Guid.Empty, Guid.Empty, BadRequest(new { error = "Missing or invalid organisation id" }));

            if (!await membershipRepository.IsMemberAsync(ownerId, organisationId, ct))
                return (Guid.Empty, Guid.Empty, Forbid());

            return (ownerId, organisationId, null);
        }
    }
}

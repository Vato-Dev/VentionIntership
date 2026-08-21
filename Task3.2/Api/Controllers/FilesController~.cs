using System.Security.Claims;
using Api.Filters;
using Infrastructure.FileManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
   // [Authorize]
    public sealed class FilesController(FileUploadService fileUploadService) : ControllerBase
    {
        [HttpPost]
        [Consumes("multipart/form-data")]
        [DisableFormValueModelBinding]
        [RequestSizeLimit(100 * 1024 * 1024)]
        public async Task<IActionResult> Upload(CancellationToken ct)
        {
   
            var (statusCode, file, error) = await fileUploadService.HandleUploadAsync(Request, Guid.NewGuid(), Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"), ct);

            if (file != null)
            {
                return StatusCode(statusCode, file);
            }

            return StatusCode(statusCode, new
            {
                error
            });
        }
    }
}

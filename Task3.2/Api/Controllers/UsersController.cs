using Application.Abstractions;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController(IUserService userService) : ControllerBase
    {
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var user = await userService.GetUserByIdAsync(id, cancellationToken);
            if (user == null) return NotFound();
            return Ok(user);
        }
 
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid? keySetId,
            [FromQuery] int? page = 1,
            [FromQuery] int? pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await userService.GetAllUsersAsync(keySetId, page, pageSize, cancellationToken);
            return Ok(result);
        }
 
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDto dto, CancellationToken cancellationToken)
        {
            await userService.UpdateUserAsync(id, dto, cancellationToken); 
            var updated = await userService.GetUserByIdAsync(id, cancellationToken);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        /*  [HttpDelete("{id:guid}")]
          public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
          {
              await userService.DeleteUserAsync(id, cancellationToken);
              return NoContent();
          }*/ // left as-is per your own note - cascade behavior on Membership/Files/Sessions
        // needs a decision before this goes live
    }
}

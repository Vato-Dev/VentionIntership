using Application.Abstractions;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto, CancellationToken cancellationToken)
        {
            var createdUser = await userService.CreateUserAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDto dto, CancellationToken cancellationToken)
        {
            await userService.UpdateUserAsync(id, dto, cancellationToken);
            return NoContent();
        }

      /*  [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await userService.DeleteUserAsync(id, cancellationToken);
            return NoContent();
        }*/ // since i don't have soft delete i won't use it for now
    }
}

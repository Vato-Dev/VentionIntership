using Application.Abstractions;
using Application.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    namespace Presentation.Controllers //Fat controller is not best practice and validation/exceptions hadling should be implemented better, but we are practicing other things now.
    {
        [ApiController]
        [Route("api/[controller]")]
        public sealed class UsersController(IUserService userService) : ControllerBase
        {
            [HttpGet]
            public async Task<IActionResult> GetAll(
                [FromQuery] int? keySetId = null, 
                [FromQuery] int? page = 1, 
                [FromQuery] int? pageSize = 10)
            {
                var pagedUsers = await userService.GetAllUsersAsync(keySetId, page, pageSize);
            
                var response = new PagedResponse<UserResponseDto>
                {
                    Data = pagedUsers.Data.Select(u => new UserResponseDto
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email,
                        PositionId = u.PositionId,
                        OrganizationId = u.OrganizationId,
                        CreatedAt = u.CreatedAt
                    }).ToList(),
                    PageNumber = pagedUsers.PageNumber,
                    PageSize = pagedUsers.PageSize,
                    TotalItems = pagedUsers.TotalItems,
                    TotalPages = pagedUsers.TotalPages,
                    LastSeenId = pagedUsers.LastSeenId
                };

                return Ok(response);
            }

            [HttpGet("{id:int}")]
            public async Task<IActionResult> GetById(int id)
            {
                var user = await userService.GetUserByIdAsync(id);
                if (user == null) return NotFound($"User with ID {id} not found.");

                var response = new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    PositionId = user.PositionId,
                    OrganizationId = user.OrganizationId,
                    CreatedAt = user.CreatedAt
                };
                return Ok(response);
            }

            [HttpPost]
            public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
            {
                var user = new User
                {
                    Username = dto.Username, Email = dto.Email, PositionId = dto.PositionId, OrganizationId = dto.OrganizationId
                };

                try
                {
                    await userService.CreateUserAsync(user);

                    var response = new UserResponseDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        PositionId = user.PositionId,
                        OrganizationId = user.OrganizationId,
                        CreatedAt = user.CreatedAt
                    };

                    return CreatedAtAction(nameof(GetById), new
                    {
                        id = response.Id
                    }, response);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            [HttpPut("{id:int}")]
            public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
            {
                if (id != dto.Id) return BadRequest("Mismatched User ID.");

                var existingUser = await userService.GetUserByIdAsync(id);
                if (existingUser == null) return NotFound($"User with ID {id} not found.");

                existingUser.Username = dto.Username;
                existingUser.Email = dto.Email;
                existingUser.PositionId = dto.PositionId;
                existingUser.OrganizationId = dto.OrganizationId;

                try
                {
                    await userService.UpdateUserAsync(existingUser);
                    return NoContent();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            [HttpDelete("{id:int}")]
            public async Task<IActionResult> Delete(int id)
            {
                try
                {
                    var user = await userService.GetUserByIdAsync(id);
                    if (user == null) return NotFound($"User with ID {id} not found.");

                    await userService.DeleteUserAsync(id);
                    return NoContent();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }
    }
}

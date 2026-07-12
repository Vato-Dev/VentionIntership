using Application.Abstractions;
using Application.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class SessionsController(ISessionService sessionService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(
            CancellationToken cancellationToken,
            [FromQuery] int? keySetId = null, 
            [FromQuery] int? page = 1, 
            [FromQuery] int? pageSize = 10)
        {
            var pagedSessions = await sessionService.GetAllSessionsAsync(keySetId, page, pageSize, cancellationToken);
            
            var response = new PagedResponse<SessionResponseDto>
            {
                Data = pagedSessions.Data.Select(s => new SessionResponseDto
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    ExpiresAt = s.ExpiresAt
                }).ToList(),
                PageNumber = pagedSessions.PageNumber,
                PageSize = pagedSessions.PageSize,
                TotalItems = pagedSessions.TotalItems,
                TotalPages = pagedSessions.TotalPages,
                LastSeenId = pagedSessions.LastSeenId
            };

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var session = await sessionService.GetSessionByIdAsync(id, cancellationToken);
            if (session == null) return NotFound($"Session with ID {id} not found.");

            var response = new SessionResponseDto
            {
                Id = session.Id,
                UserId = session.UserId,
                IsActive = session.IsActive,
                CreatedAt = session.CreatedAt,
                ExpiresAt = session.ExpiresAt
            };
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SessionCreateDto dto, CancellationToken cancellationToken)
        {
            var session = new Session
            {
                UserId = dto.UserId,
                ExpiresAt = dto.ExpiresAt
            };

            try
            {
                await sessionService.CreateSessionAsync(session, cancellationToken);

                var response = new SessionResponseDto
                {
                    Id = session.Id,
                    UserId = session.UserId,
                    IsActive = session.IsActive,
                    CreatedAt = session.CreatedAt,
                    ExpiresAt = session.ExpiresAt
                };

                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SessionUpdateDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.Id) return BadRequest("Mismatched Session ID.");

            var existingSession = await sessionService.GetSessionByIdAsync(id, cancellationToken);
            if (existingSession == null) return NotFound($"Session with ID {id} not found.");

            existingSession.IsActive = dto.IsActive;
            existingSession.ExpiresAt = dto.ExpiresAt;

            try
            {
                await sessionService.UpdateSessionAsync(existingSession, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                var session = await sessionService.GetSessionByIdAsync(id, cancellationToken);
                if (session == null) return NotFound($"Session with ID {id} not found.");

                await sessionService.DeleteSessionAsync(id, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
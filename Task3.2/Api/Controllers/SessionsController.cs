using Application.Abstractions;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionsController(ISessionService sessionService) : ControllerBase
    {
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var session = await sessionService.GetSessionByIdAsync(id, cancellationToken);
            if (session == null) return NotFound();
            return Ok(session);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? keySetId, 
            [FromQuery] int? page = 1, 
            [FromQuery] int? pageSize = 10, 
            CancellationToken cancellationToken = default)
        {
            var result = await sessionService.GetAllSessionsAsync(keySetId, page, pageSize, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SessionCreateDto dto, CancellationToken cancellationToken)
        {
            var createdSession = await sessionService.CreateSessionAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = int.Parse(createdSession.Id) }, createdSession);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SessionUpdateDto dto, CancellationToken cancellationToken)
        {
            await sessionService.UpdateSessionAsync(id, dto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await sessionService.DeleteSessionAsync(id, cancellationToken);
            return NoContent();
        }
    }
}

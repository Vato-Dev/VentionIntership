using Application.Abstractions;
using Application.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
  [ApiController]
    [Route("api/[controller]")]
    public sealed class PositionsController : ControllerBase
    {
        private readonly IPositionService _positionService;

        public PositionsController(IPositionService positionService) => _positionService = positionService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var positions = await _positionService.GetAllPositionsAsync();
            var response = positions.Select(p => new PositionResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description
            });
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var position = await _positionService.GetPositionByIdAsync(id);
            if (position == null) return NotFound($"Position with ID {id} not found.");

            var response = new PositionResponseDto
            {
                Id = position.Id,
                Title = position.Title,
                Description = position.Description
            };
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PositionCreateDto dto)
        {
            var position = new Position
            {
                Title = dto.Title,
                Description = dto.Description
            };

            try
            {
                await _positionService.CreatePositionAsync(position);
                
                var response = new PositionResponseDto
                {
                    Id = position.Id,
                    Title = position.Title,
                    Description = position.Description
                };

                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PositionUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest("Mismatched Position ID.");

            var existingPosition = await _positionService.GetPositionByIdAsync(id);
            if (existingPosition == null) return NotFound($"Position with ID {id} not found.");

            existingPosition.Title = dto.Title;
            existingPosition.Description = dto.Description;

            try
            {
                await _positionService.UpdatePositionAsync(existingPosition);
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
                var position = await _positionService.GetPositionByIdAsync(id);
                if (position == null) return NotFound($"Position with ID {id} not found.");

                await _positionService.DeletePositionAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

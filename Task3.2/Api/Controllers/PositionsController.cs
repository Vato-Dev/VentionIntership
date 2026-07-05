using Application.Abstractions;
using Application.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class PositionsController(IPositionService positionService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(
            CancellationToken cancellationToken,
            [FromQuery] int? keySetId = null, 
            [FromQuery] int? page = 1, 
            [FromQuery] int? pageSize = 10)
        {
            var pagedPositions = await positionService.GetAllPositionsAsync(keySetId, page, pageSize, cancellationToken);
            
            var response = new PagedResponse<PositionResponseDto>
            {
                Data = pagedPositions.Data.Select(p => new PositionResponseDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description
                }).ToList(),
                PageNumber = pagedPositions.PageNumber,
                PageSize = pagedPositions.PageSize,
                TotalItems = pagedPositions.TotalItems,
                TotalPages = pagedPositions.TotalPages,
                LastSeenId = pagedPositions.LastSeenId
            };

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var position = await positionService.GetPositionByIdAsync(id, cancellationToken);
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
        public async Task<IActionResult> Create([FromBody] PositionCreateDto dto, CancellationToken cancellationToken)
        {
            var position = new Position
            {
                Title = dto.Title,
                Description = dto.Description
            };

            try
            {
                await positionService.CreatePositionAsync(position, cancellationToken);
                
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
        public async Task<IActionResult> Update(int id, [FromBody] PositionUpdateDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.Id) return BadRequest("Mismatched Position ID.");

            var existingPosition = await positionService.GetPositionByIdAsync(id, cancellationToken);
            if (existingPosition == null) return NotFound($"Position with ID {id} not found.");

            existingPosition.Title = dto.Title;
            existingPosition.Description = dto.Description;

            try
            {
                await positionService.UpdatePositionAsync(existingPosition, cancellationToken);
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
                var position = await positionService.GetPositionByIdAsync(id, cancellationToken);
                if (position == null) return NotFound($"Position with ID {id} not found.");

                await positionService.DeletePositionAsync(id, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
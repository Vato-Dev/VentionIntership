using LibraryManager.Application.DTO_s.Requests;
using LibraryManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReadersController(IReaderService readerService) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> Create([FromBody] AddReaderRequest request, CancellationToken ct)
        {
         return  await readerService.CreateReader(request, ct);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update([FromBody] UpdateReaderRequest request, CancellationToken ct)
        {
            await readerService.UpdateReaderProfile(request, ct);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await readerService.DeleteReader(id, ct);
            return NoContent();
        }

        [HttpPost("{id:int}/block")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Block(int id, CancellationToken ct)
        {
            await readerService.BlockReader(id, ct);
            return NoContent();
        }
    }
}

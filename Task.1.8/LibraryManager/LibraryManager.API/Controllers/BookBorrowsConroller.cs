using LibraryManager.Application.Interfaces;
using LibraryManager.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookBorrowsController(IBookBorrowService borrowService) : ControllerBase
    {
        [HttpPost("borrow")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Borrow([FromQuery] int bookId, [FromQuery] int readerId, CancellationToken ct)
        {
            await borrowService.BorrowBook(bookId, readerId, ct);
            return Ok();
        }

        [HttpPost("return")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Return([FromBody] ReturnBookRequest request, CancellationToken ct)
        {
            await borrowService.ReturnBook(
            request.BorrowingId, 
            request.ReturnDate, 
            request.AboutCondition, 
            request.ConditionAtReturn, 
            ct);
                
            return NoContent();
        }
    }

    public sealed record ReturnBookRequest(
        int BorrowingId, 
        DateTime ReturnDate, 
        string? AboutCondition, 
        BookCondition ConditionAtReturn);
}

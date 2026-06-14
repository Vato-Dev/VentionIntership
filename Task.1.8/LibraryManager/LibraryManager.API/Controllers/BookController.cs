using LibraryManager.Application.DTO_s.Requests;
using LibraryManager.Application.Interfaces.Books;
using LibraryManager.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController(IBookService bookService) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Add([FromBody] AddBookRequest request, CancellationToken ct)
        {
            await bookService.AddBook(request, ct);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(BookInfoDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<BookInfoDto>> GetInfo(int id, CancellationToken ct)
        {
            var bookInfo = await bookService.GetBookInfo(id, ct);
            return Ok(bookInfo);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateInfo([FromBody] UpdateBookRequest request, CancellationToken ct)
        {
            await bookService.UpdateBookInfo(request, ct);
            return NoContent();
        }

        [HttpPatch("{bookId:int}/isbn")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateIsbn(int bookId, [FromBody] string newIsbn, CancellationToken ct)
        {
            await bookService.UpdateIsbn(bookId, newIsbn, ct);
            return NoContent();
        }
    }
}

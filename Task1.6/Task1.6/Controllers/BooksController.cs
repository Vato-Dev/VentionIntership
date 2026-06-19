using Microsoft.AspNetCore.Mvc;
using Task1._6.Abstractions;
using Task1._6.DTOs;
using Task1._6.Services;

namespace Task1._6.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController(IBookService bookService) : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<BookDto>> GetAll()
        {
            return Ok(bookService.GetAll());
        }

        [HttpGet("{id:int}")]
        public ActionResult<BookDto> GetById(int id)
        {
            var book = bookService.GetById(id);
            if (book is null) return NotFound();
        
            return Ok(book);
        }

        [HttpPost]
        public ActionResult<BookDto> Create([FromBody] CreateBookDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("Book title is required");

            var createdBook = bookService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdBook.Id }, createdBook);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] UpdateBookDto dto)
        {
            var updated = bookService.Update(id, dto);
            if (!updated) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var deleted = bookService.Delete(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}

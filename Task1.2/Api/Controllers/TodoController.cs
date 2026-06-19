using Api.DTOs;
using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
  [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private static readonly List<Todo> Todos = new();
        private static int _nextId = 1; 

        [HttpGet]
        public ActionResult<List<Todo>> GetAll()
        {
            if (Todos.Count == 0)
                return NoContent();

            return Ok(Todos);
        }

        [HttpGet("{id:int}")]
        public ActionResult<Todo> GetById(int id)
        {
            var todo = Todos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
                return NotFound(); 

            return Ok(todo);
        }

        [HttpPost]
        public ActionResult<Todo> Create([FromBody] CreateTodoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Description))
                return BadRequest("Description cannot be empty.");

            int newId = _nextId++;
            
            var newTodo = Todo.CreateTodo(newId, dto.Description);
            Todos.Add(newTodo);

            return CreatedAtAction(nameof(GetById), new { id = newTodo.Id }, newTodo);
        }

        [HttpPut("{id:int}")]
        public ActionResult Update(int id, [FromBody] UpdateTodoDto dto)
        {
            var todo = Todos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(dto.Description))
                return BadRequest("Description cannot be empty.");

            todo.update(dto.Description);

            return NoContent(); 
        }

        //Now when i'm done with crud i'm thinking about logic since it's a simple task i won't make an service or more projects i'm choosing from patch and post updating of model
        [HttpPatch("{id:int}/status")]
        public ActionResult ChangeStatus(int id, [FromBody] ChangeTodoStatusDto dto)
        {
            var todo = Todos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
                return NotFound();

            try
            {
                switch (dto.Status)
                {
                    case Status.Completed:
                        todo.complete();
                        break;
                    case Status.Cancelled:
                        todo.cancel();
                        break;
                    case Status.Active:
                        return BadRequest("Cannot revert task back to Active.");
                    default:
                        return BadRequest("Unknown status.");
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var todo = Todos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
                return NotFound();

            Todos.Remove(todo);

            return NoContent(); 
        } //we can move the repeating logic into private method for example searching in list/db and throwing not found
        //Returning results is better since we do not throw a true exception it's an helper object embeded in Asp.Net
    }
}

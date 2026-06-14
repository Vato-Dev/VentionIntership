using Api.Models;

namespace Api.DTOs
{
    public sealed record CreateTodoDto(string Description); //it's immutable after creation and their equality is defined by comparing the values not references
    public sealed record UpdateTodoDto(string Description);
    public sealed record ChangeTodoStatusDto(Status Status);
    
}

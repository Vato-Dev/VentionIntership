namespace Domain.Models
{
    public sealed class Position
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } 
        
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}

namespace Domain.Models
{
    public sealed class Session
    {
        public int Id { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
        public string UserId { get; set; }  = string.Empty;
        public User User { get; set; } = null!;
    }
}

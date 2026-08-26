namespace Domain.Models
{
    public sealed class Session : DomainEntity<int>
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid UserId { get; set; } 
        public User User { get; set; } = null!;
    }
}

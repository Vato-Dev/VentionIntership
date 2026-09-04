namespace Domain.Models
{
    public sealed class UserChat : DomainEntity<Guid>
    {
        public Guid UserId1 { get; set; }
        public User User1 { get; set; } = null!;
    
        public Guid UserId2 { get; set; }
        public User User2 { get; set; } = null!;
    
        public string LastMessage { get; set; } = string.Empty;
        public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;
        public int UnreadCount { get; set; }
    
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    
        public ICollection<UserChatMessage> Messages { get; set; } = new List<UserChatMessage>();
    }
    
    public sealed class UserChatMessage : DomainEntity<Guid>
    {
        public Guid ChatId { get; set; }
        public UserChat Chat { get; set; } = null!;
    
        public Guid SenderId { get; set; }
        public User Sender { get; set; } = null!;
    
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }
    }
}

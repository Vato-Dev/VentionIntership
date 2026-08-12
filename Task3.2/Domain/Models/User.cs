using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public sealed class User 
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; 
        public string PasswordHash { get; set; } = string.Empty;
        public int PositionId { get; set; }
        
        public Position Position { get; set; } = null!;
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

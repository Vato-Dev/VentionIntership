using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public sealed class User 
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        
        public int PositionId { get; set; }
        public int OrganizationId { get; set; }
        
        public Position Position { get; set; } = null!;
        public Organization Organization { get; set; } = null!;
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public sealed class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty; 
        
        public ICollection<User> Users { get; set; } = new List<User>(); // i won't make an encapsulation though making backing field and readonly property it's overkill
    }

    public sealed class Position
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } 
        
        public ICollection<User> Users { get; set; } = new List<User>();
    }

    public sealed class Session
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}

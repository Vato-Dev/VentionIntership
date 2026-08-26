namespace Domain.Models
{
    public sealed class Membership: DomainEntity<Guid>
    {
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }

        public string Role { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public Organization Organization { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}

namespace Domain.Models
{
    public sealed class Membership
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string OrganizationId { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public Organization Organization { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}

namespace Domain.Models
{
    public sealed class Organization: DomainEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string StreetAddress { get; set; } = string.Empty;
        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    }
}

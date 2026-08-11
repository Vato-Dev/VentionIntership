namespace Domain.Models
{
    public sealed class Organization
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string StreetAddress { get; set; } = string.Empty;
        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    }
}

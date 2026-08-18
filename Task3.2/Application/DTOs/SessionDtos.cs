namespace Application.DTOs
{
    public sealed record SessionCreateDto
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    public sealed record SessionUpdateDto
    {
        public string Id { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public sealed record SessionResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } =  string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}

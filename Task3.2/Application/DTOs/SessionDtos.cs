namespace Application.DTOs
{
    public sealed class SessionCreateDto
    {
        public int UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public sealed class SessionUpdateDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public sealed class SessionResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}

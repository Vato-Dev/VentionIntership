namespace Application.DTOs
{
    public sealed class UserCreateDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int PositionId { get; set; }
        public int OrganizationId { get; set; }
    }

    public sealed class UserUpdateDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int PositionId { get; set; }
        public int OrganizationId { get; set; }
    }

    public sealed class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int PositionId { get; set; }
        public int OrganizationId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

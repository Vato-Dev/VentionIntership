using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public sealed record UserCreateDto
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public sealed record UserUpdateDto
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Password { get; set; } 
    }

    public sealed record UserResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        [property: JsonPropertyName("organisations")]
        public List<UserOrganizationMembershipDto> Organisations { get; set; } = [];
    }
    public sealed record UserOrganizationMembershipDto
    {
        public string Id { get; set; } = string.Empty; 
        public string Name { get; set; } = string.Empty; 
        public string Role { get; set; } = string.Empty;
    }

}

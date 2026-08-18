using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public sealed record OrganizationCreateDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public sealed record OrganizationUpdateDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public sealed record OrganizationResponseDto
    {   
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

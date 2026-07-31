using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public sealed class OrganizationCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty;
    }

    public sealed class OrganizationUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty;
    }

    public sealed class OrganizationResponseDto
    {   
        [JsonNumberHandling(JsonNumberHandling.WriteAsString)] 

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty;
    }
}

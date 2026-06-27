namespace Application.DTOs
{
    public sealed class PositionCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public sealed class PositionUpdateDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public sealed class PositionResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}

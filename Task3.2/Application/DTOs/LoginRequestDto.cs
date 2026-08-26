namespace Application.DTOs
{
    public sealed record LoginResponseDto(
        string Id,
        string Email,
        string Name,
        string Role,
        string? Image,
        string AccessToken);
}

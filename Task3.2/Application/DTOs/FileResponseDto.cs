namespace Application.DTOs
{
    public sealed record FileResponseDto(
        Guid Id,
        string Filename,
        long Size,
        string Status,
        string ContentType,
        string Checksum,
        string StorageKey,
        Guid OrganisationId,
        Guid OwnerId,
        string? Application,
        string? ProcessingError,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}

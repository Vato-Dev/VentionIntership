namespace Application.Messages;

public sealed record FileProcessingRequested
{
    public Guid FileId { get; init; }
    public Guid OrganisationId { get; init; }
    public string StorageKey { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public string Filename { get; init; } = string.Empty;

    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}

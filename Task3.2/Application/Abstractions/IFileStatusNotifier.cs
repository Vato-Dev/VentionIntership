namespace Application.Abstractions;


//just interface i'll give to FileProcessingConsumer , no SignalR types will be heere , implementation will be in Api and i'll register it in DI
public interface IFileStatusNotifier
{
    Task NotifyStatusChangedAsync(
        Guid organisationId, Guid fileId, string status, string? processingError, DateTime updatedAt, CancellationToken ct);

    Task NotifyAsync(Guid organisationId, string title, string message, CancellationToken ct);
}

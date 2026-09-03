using Api.Hubs;
using Application.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs;

// The only place in the whole app that knows FileProcessingConsumer's status updates end
// up going out over SignalR specifically - Infrastructure just calls IFileStatusNotifier
// and doesn't care how it's actually delivered.
public sealed class SignalRFileStatusNotifier(IHubContext<FileStatusHub> hubContext) : IFileStatusNotifier
{
    public Task NotifyStatusChangedAsync(Guid organisationId, Guid fileId, string status, string? processingError, DateTime updatedAt, CancellationToken ct) =>
        hubContext.Clients.Group(FileStatusHub.GroupName(organisationId)).SendAsync(
        "FileStatusChanged", new { fileId, status, processingError, updatedAt }, ct);

    public Task NotifyAsync(Guid organisationId, string title, string message, CancellationToken ct) =>
        hubContext.Clients.Group(FileStatusHub.GroupName(organisationId)).SendAsync(
        "Notification", new { title, message }, ct);
}

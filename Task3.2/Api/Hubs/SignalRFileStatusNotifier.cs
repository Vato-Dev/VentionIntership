using Api.Hubs;
using Application.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace Api.Hubs;

public sealed class SignalRFileStatusNotifier(
    IHubContext<FileStatusHub> hubContext,
    ILogger<SignalRFileStatusNotifier> logger) : IFileStatusNotifier
{
    public Task NotifyStatusChangedAsync(Guid organisationId, Guid fileId, string status, string? processingError, DateTime updatedAt, CancellationToken ct)
    {
        var groupName = FileStatusHub.GroupName(organisationId);
        logger.LogInformation("Sending to group: {GroupName}", groupName);
        logger.LogInformation("FileId: {FileId}, Status: {Status}", fileId, status);
        
        return hubContext.Clients.Group(groupName).SendAsync(
        "FileStatusChanged", new { fileId, status, processingError, updatedAt }, ct);
    }

    public Task NotifyAsync(Guid organisationId, string title, string message, CancellationToken ct)
    {
        var groupName = FileStatusHub.GroupName(organisationId);
        logger.LogInformation("Notification to {GroupName}: {Title}", groupName, title);
        
        return hubContext.Clients.Group(groupName).SendAsync(
        "Notification", new { title, message }, ct);
    }
}

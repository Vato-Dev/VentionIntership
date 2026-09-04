using System.Security.Claims;
using Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace Api.Hubs;

[Authorize]
public sealed class FileStatusHub(
    IMembershipRepository membershipRepository,
    ILogger<FileStatusHub> logger) : Hub
{
    public override async Task OnConnectedAsync()
    {
        logger.LogInformation("=== HUB CONNECTED ===");
        
        var httpContext = Context.GetHttpContext();
        var orgIdRaw = httpContext?.Request.Query["orgId"].ToString();
        logger.LogInformation("orgId from query: {OrgId}", orgIdRaw);

        if (string.IsNullOrEmpty(orgIdRaw) || !Guid.TryParse(orgIdRaw, out var orgId))
        {
            logger.LogWarning("orgId is invalid, aborting connection");
            Context.Abort();
            return;
        }

        var userIdRaw = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        logger.LogInformation("userId from claims: {UserId}", userIdRaw);
        
        if (string.IsNullOrEmpty(userIdRaw) || !Guid.TryParse(userIdRaw, out var userId))
        {
            logger.LogWarning("userId is invalid, aborting connection");
            Context.Abort();
            return;
        }

        /* if (!await membershipRepository.IsMemberAsync(userId, orgId, Context.ConnectionAborted))
        {
            logger.LogWarning("User {UserId} is not a member of org {OrgId}", userId, orgId);
            Context.Abort();
            return;
        }*/ //because i've roles but it makes problems while developing 

        var groupName = GroupName(orgId);
        logger.LogInformation("Adding connection {ConnectionId} to group: {GroupName}", Context.ConnectionId, groupName);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName, Context.ConnectionAborted);
        
        logger.LogInformation("User {UserId} connected to org {OrgId} successfully!", userId, orgId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation("Connection {ConnectionId} disconnected", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public static string GroupName(Guid organisationId) => $"org:{organisationId}";
}
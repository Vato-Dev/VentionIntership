using System.Security.Claims;
using Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs;

[Authorize]
public sealed class FileStatusHub(IMembershipRepository membershipRepository) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var orgIdRaw = httpContext?.Request.Query["orgId"].ToString();

        if (string.IsNullOrEmpty(orgIdRaw) || !Guid.TryParse(orgIdRaw, out var orgId))
        {
            Context.Abort();
            return;
        }

        var userIdRaw = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdRaw, out var userId))
        {
            Context.Abort();
            return;
        }

        if (!await membershipRepository.IsMemberAsync(userId, orgId, Context.ConnectionAborted))
        {
            Context.Abort();
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(orgId), Context.ConnectionAborted);
        await base.OnConnectedAsync();
    }

    public static string GroupName(Guid organisationId) => $"org:{organisationId}";
}

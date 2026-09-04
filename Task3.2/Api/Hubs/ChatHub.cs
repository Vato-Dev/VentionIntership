using System.Security.Claims;
using Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs;

[Authorize]
public sealed class ChatHub(PresenceTracker presence, IUserChatMessageRepository messageRepository) : Hub
{
    private const string PresenceGroup = "presence";

    public override async Task OnConnectedAsync()
    {
        var userIdRaw = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdRaw, out var userId))
        {
            Context.Abort();
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(userId), Context.ConnectionAborted);
        await Groups.AddToGroupAsync(Context.ConnectionId, PresenceGroup, Context.ConnectionAborted);
        if (presence.UserConnected(userId, Context.ConnectionId))
        {
            await Clients.Group(PresenceGroup).SendAsync("UserOnline", userId, Context.ConnectionAborted);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userIdRaw = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdRaw, out var userId))
        {
            if (presence.UserDisconnected(userId, Context.ConnectionId))
            {
                await Clients.Group(PresenceGroup).SendAsync("UserOffline", userId);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    public Guid[] GetOnlineUsers() => presence.GetOnlineUsers();


    public async Task<List<ChatMessageNotification>> ReplayMissedMessages(DateTime since)
    {
        var userIdRaw = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdRaw, out var userId)) return [];

        return await messageRepository.GetMessagesForUserSinceAsync(userId, since, Context.ConnectionAborted);
    }

    public static string GroupName(Guid userId) => $"user:{userId}";
}

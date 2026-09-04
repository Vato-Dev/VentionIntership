using Application.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs
{
    public sealed class SignalRChatNotifier(IHubContext<ChatHub> hubContext ) : IChatNotifier
    {
        public Task NotifyNewMessageAsync(Guid recipientUserId, ChatMessageNotification message, CancellationToken ct = default) =>
            hubContext.Clients.Group(ChatHub.GroupName(recipientUserId)).SendAsync("NewMessage", message, ct);
    }

}

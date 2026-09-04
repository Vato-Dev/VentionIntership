using Application.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Api.Hubs
{
    public sealed class SignalRChatNotifier(
        IHubContext<ChatHub> hubContext,
        ILogger<SignalRChatNotifier> logger) : IChatNotifier
    {
        public async Task NotifyNewMessageAsync(Guid recipientUserId, ChatMessageNotification message, CancellationToken ct = default)
        {
            logger.LogInformation("SignalRChatNotifier called for user {UserId}", recipientUserId);
            logger.LogInformation("Message: {Message}", message.Content);

            if (hubContext == null)
            {
                logger.LogWarning("⚠hubContext is null!");
                return; }

            var groupName = ChatHub.GroupName(recipientUserId);
            logger.LogInformation("Sending to group: {GroupName}", groupName);

            await hubContext.Clients
                .Group(groupName)
                .SendAsync("NewMessage", message, ct);

            logger.LogInformation("Notification sent successfully");
        }
    }
}

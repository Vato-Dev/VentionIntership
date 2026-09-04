using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs
{
    public class ChatHub(ILogger<ChatHub> logger) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userIdRaw = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdRaw, out var userId))
            {
                Context.Abort();
                return;
            }
 
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(userId), Context.ConnectionAborted);
            await base.OnConnectedAsync();
        }
 
        public static string GroupName(Guid userId) => $"user:{userId}";
    }
}

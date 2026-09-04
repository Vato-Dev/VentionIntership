using Domain.Models;

namespace Application.Abstractions
{
    public interface IUserChatRepository : IBaseRepository<UserChat, Guid>
    {
        Task<List<UserChat>> GetUserChatsAsync(Guid userId, CancellationToken ct = default);
        Task<UserChat?> GetChatBetweenUsersAsync(Guid userId1, Guid userId2, CancellationToken ct = default);
        Task<UserChat?> GetChatWithMessagesAsync(Guid chatId, Guid userId, CancellationToken ct = default);
    }
    
    public interface IUserChatMessageRepository : IBaseRepository<UserChatMessage, Guid>
    {
        Task<List<UserChatMessage>> GetMessagesByChatIdAsync(Guid chatId, int limit = 100, CancellationToken ct = default);
        Task MarkMessagesAsReadAsync(Guid chatId, Guid userId, CancellationToken ct = default);
        Task<int> CountUnreadForUserAsync(Guid chatId, Guid userId, CancellationToken ct = default);
        Task<List<ChatMessageNotification>> GetMessagesForUserSinceAsync(Guid userId, DateTime since, CancellationToken ct = default);
    }
}

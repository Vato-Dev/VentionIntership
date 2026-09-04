using Domain.Models;

namespace Application.Abstractions
{
    public interface IUserChatService
    {
        Task<List<UserChat>> GetUserChatsAsync(Guid userId, CancellationToken ct = default);
        Task<UserChat> CreateOrGetChatAsync(Guid userId1, Guid userId2, CancellationToken ct = default);
        Task<UserChat?> GetChatWithMessagesAsync(Guid chatId, Guid userId, CancellationToken ct = default);
        Task<UserChatMessage> SendMessageAsync(Guid chatId, Guid senderId, string content, CancellationToken ct = default);
        Task<bool> DeleteChatAsync(Guid chatId, Guid userId, CancellationToken ct = default);
    }
}

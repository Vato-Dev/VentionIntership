using Application.Abstractions;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class UserChatMessageRepository(AppDbContext context) : BaseRepository<UserChatMessage, Guid>(context), IUserChatMessageRepository
{
    public async Task<List<UserChatMessage>> GetMessagesByChatIdAsync(Guid chatId, int limit = 100, CancellationToken ct = default)
    {
        return await DbSet
            .Where(m => m.ChatId == chatId)
            .Include(m => m.Sender)
            .OrderBy(m => m.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task MarkMessagesAsReadAsync(Guid chatId, Guid userId, CancellationToken ct = default)
    {
        await DbSet
            .Where(m => m.ChatId == chatId && m.SenderId != userId && !m.IsRead)
            .ExecuteUpdateAsync(setters => setters.SetProperty(m => m.IsRead, true), ct);
    }
    public Task<int> CountUnreadForUserAsync(Guid chatId, Guid userId, CancellationToken ct = default) =>
        context.UserChatMessages.CountAsync(
        m => m.ChatId == chatId && m.SenderId != userId && !m.IsRead, ct);
    
    public async Task<List<ChatMessageNotification>> GetMessagesForUserSinceAsync(
        Guid userId, DateTime since, CancellationToken ct = default)
    {
        var chatIds = context.UserChats
            .Where(c => c.UserId1 == userId || c.UserId2 == userId)
            .Select(c => c.Id);

        return await DbSet
            .Where(m => chatIds.Contains(m.ChatId) && m.CreatedAt > since && m.SenderId != userId)
            .Include(m => m.Sender)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new ChatMessageNotification(m.Id, m.ChatId, m.Content, m.SenderId, m.Sender.Name, m.CreatedAt))
            .ToListAsync(ct);
    }
}

using Application.Abstractions;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class UserChatRepository(AppDbContext context) : BaseRepository<UserChat, Guid>(context), IUserChatRepository
{
    public async Task<List<UserChat>> GetUserChatsAsync(Guid userId, CancellationToken ct = default)
    {
        return await DbSet
            .Where(c => c.UserId1 == userId || c.UserId2 == userId)
            .Include(c => c.User1)
            .Include(c => c.User2)
            .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync(ct);
    }

    public async Task<UserChat?> GetChatBetweenUsersAsync(Guid userId1, Guid userId2, CancellationToken ct = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(c =>
                (c.UserId1 == userId1 && c.UserId2 == userId2) ||
                (c.UserId1 == userId2 && c.UserId2 == userId1), ct);
    }

    public async Task<UserChat?> GetChatWithMessagesAsync(Guid chatId, Guid userId, CancellationToken ct = default)
    {
        return await DbSet
            .Include(c => c.User1)
            .Include(c => c.User2)
            .Include(c => c.Messages.OrderBy(m => m.CreatedAt))
            .FirstOrDefaultAsync(c => c.Id == chatId && (c.UserId1 == userId || c.UserId2 == userId), ct);
    }
}

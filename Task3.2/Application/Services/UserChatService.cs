using System.Data;
using Application.Abstractions;
using Domain.Models;

namespace Application.Services;

public class UserChatService(
    IUserChatRepository chatRepository,
    IUserChatMessageRepository messageRepository,
    IBaseRepository<User, Guid> userRepository, 
    IUnitOfWork unitOfWork) : IUserChatService
{
    public async Task<List<UserChat>> GetUserChatsAsync(Guid userId, CancellationToken ct = default)
    {
        var chats = await chatRepository.GetUserChatsAsync(userId, ct);

       
        foreach (var chat in chats)
        {
            chat.UnreadCount = await messageRepository.CountUnreadForUserAsync(chat.Id, userId, ct);
        }

        return chats;
    }

    public async Task<UserChat> CreateOrGetChatAsync(Guid userId1, Guid userId2, CancellationToken ct = default)
    {
        if (userId1 == userId2)
            throw new InvalidOperationException("Cannot create chat with yourself");

        var existing = await chatRepository.GetChatBetweenUsersAsync(userId1, userId2, ct);
        if (existing != null)
            return existing;
        
        var user1 = await userRepository.GetByIdAsync(userId1, ct)
            ?? throw new InvalidOperationException("User not found");
        var user2 = await userRepository.GetByIdAsync(userId2, ct)
            ?? throw new InvalidOperationException("User not found");

        var chat = new UserChat
        {
            Id = Guid.NewGuid(),
            UserId1 = userId1,
            UserId2 = userId2,
            User1 = user1,
            User2 = user2,
            LastMessage = "Chat created",
            LastMessageAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
        try
        {
            await chatRepository.AddAsync(chat, ct);
            await unitOfWork.SaveChangesAsync(ct);
            await unitOfWork.CommitTransactionAsync(ct);
            return chat;
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task<UserChat?> GetChatWithMessagesAsync(Guid chatId, Guid userId, CancellationToken ct = default)
    {
        var chat = await chatRepository.GetChatWithMessagesAsync(chatId, userId, ct);
        if (chat != null)
        {
            await messageRepository.MarkMessagesAsReadAsync(chatId, userId, ct);
            await unitOfWork.SaveChangesAsync(ct); 
        }
        return chat;
    }

    public async Task<UserChatMessage> SendMessageAsync(Guid chatId, Guid senderId, string content, CancellationToken ct = default)
    {
        var chat = await chatRepository.GetByIdAsync(chatId, ct);
        if (chat == null)
            throw new InvalidOperationException("Chat not found");

        if (chat.UserId1 != senderId && chat.UserId2 != senderId)
            throw new InvalidOperationException("You are not a participant of this chat");

        var message = new UserChatMessage
        {
            Id = Guid.NewGuid(),
            ChatId = chatId,
            SenderId = senderId,
            Content = content,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
        try
        {
            await messageRepository.AddAsync(message, ct);

            chat.LastMessage = content;
            chat.LastMessageAt = DateTime.UtcNow;
            chat.UpdatedAt = DateTime.UtcNow;
        
            chatRepository.Update(chat);
            await unitOfWork.SaveChangesAsync(ct);
            await unitOfWork.CommitTransactionAsync(ct);

            return message;
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task<bool> DeleteChatAsync(Guid chatId, Guid userId, CancellationToken ct = default)
    {
        var chat = await chatRepository.GetByIdAsync(chatId, ct);
        if (chat == null)
            return false;

        if (chat.UserId1 != userId && chat.UserId2 != userId)
            return false;

        await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
        try
        {
            chatRepository.Delete(chat);
            await unitOfWork.SaveChangesAsync(ct);
            await unitOfWork.CommitTransactionAsync(ct);
            return true;
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(CancellationToken.None);
            throw;
        }
    }
}
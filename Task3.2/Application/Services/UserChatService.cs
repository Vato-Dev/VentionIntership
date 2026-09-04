using Application.Abstractions;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class UserChatService(
    IUserChatRepository chatRepository,
    IUserChatMessageRepository messageRepository,
    IChatNotifier chatNotifier,
    ILogger<UserChatService> logger,
    IUnitOfWork unitOfWork)
    : IUserChatService
{
    public async Task<List<UserChat>> GetUserChatsAsync(Guid userId, CancellationToken ct = default)
    {
        return await chatRepository.GetUserChatsAsync(userId, ct);
    }

    public async Task<UserChat> CreateOrGetChatAsync(Guid userId1, Guid userId2, CancellationToken ct = default)
    {
        if (userId1 == userId2)
            throw new InvalidOperationException("Cannot create chat with yourself");

        var existing = await chatRepository.GetChatBetweenUsersAsync(userId1, userId2, ct);
        if (existing != null)
            return existing;

        var chat = new UserChat
        {
            Id = Guid.NewGuid(),
            UserId1 = userId1,
            UserId2 = userId2,
            LastMessage = "Chat created",
            LastMessageAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await chatRepository.AddAsync(chat, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return chat;
    }

    public async Task<UserChat?> GetChatWithMessagesAsync(Guid chatId, Guid userId, CancellationToken ct = default)
    {
        var chat = await chatRepository.GetChatWithMessagesAsync(chatId, userId, ct);
        if (chat != null)
        {
            await messageRepository.MarkMessagesAsReadAsync(chatId, userId, ct);
        }
        return chat;
    }

    public async Task<UserChatMessage> SendMessageAsync(
        Guid chatId,
        Guid senderId,
        string content,
        CancellationToken ct = default)
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

        await messageRepository.AddAsync(message, ct);

        chat.LastMessage = content;
        chat.LastMessageAt = DateTime.UtcNow;
        chat.UpdatedAt = DateTime.UtcNow;
        chat.UnreadCount++;

        chatRepository.Update(chat);

        await unitOfWork.SaveChangesAsync(ct);

        if (chatNotifier != null)
        {
            try
            {
                var recipientUserId = chat.UserId1 == senderId ? chat.UserId2 : chat.UserId1;
                logger.LogInformation("📨 Sending notification to user {UserId}", recipientUserId);

                var notification = new ChatMessageNotification(
                    message.Id,
                    message.ChatId,
                    message.Content,
                    message.SenderId,
                    message.Sender?.Name ?? "",
                    message.CreatedAt
                );
                await chatNotifier.NotifyNewMessageAsync(recipientUserId, notification, ct);
                logger.LogInformation("Notification sent successfully");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "SignalR notification failed");
            }
        }
        else
        {
            logger.LogWarning("chatNotifier is null");
        }

        return message;
    }

    public async Task<bool> DeleteChatAsync(Guid chatId, Guid userId, CancellationToken ct = default)
    {
        var chat = await chatRepository.GetByIdAsync(chatId, ct);
        if (chat == null)
            return false;

        if (chat.UserId1 != userId && chat.UserId2 != userId)
            return false;

        chatRepository.Delete(chat);
        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}

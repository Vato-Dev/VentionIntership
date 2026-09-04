namespace Application.Abstractions
{
    public interface IChatNotifier
    {
        Task NotifyNewMessageAsync(Guid recipientUserId, ChatMessageNotification message, CancellationToken ct = default);
    }
 
    public sealed record ChatMessageNotification(
        Guid Id, Guid ChatId, string Content, Guid SenderId, string SenderName, DateTime CreatedAt);

}

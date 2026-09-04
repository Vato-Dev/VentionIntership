using Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Application.DTOs;

namespace Api.Controllers;

[ApiController]
[Route("api/chats")]
[Authorize]
public class UserChatController(IUserChatService chatService,IUserRepository repository,ILogger<UserChatController> logger) : ControllerBase
{
    private Guid GetUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userId, out var id) ? id : Guid.Empty;
    }

    [HttpGet("sessions")]
    public async Task<IActionResult> GetChats(CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var chats = await chatService.GetUserChatsAsync(userId, ct);

        var result = chats.Select(chat =>
        {
            var participant = chat.UserId1 == userId ? chat.User2 : chat.User1;
            return new
            {
                id = chat.Id,
                participant = new
                {
                    id = participant.Id,
                    name = participant.Name
                },
                lastMessage = chat.LastMessage,
                lastMessageAt = chat.LastMessageAt,
                unreadCount = chat.UnreadCount
            };
        });

        return Ok(result);
    }

    [HttpPost("sessions")]
    public async Task<IActionResult> CreateChat([FromBody] CreateChatRequest request, CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        try
        {
            var user2 = await repository.GetByIdAsync(request.UserId, ct);
            if (user2 == null)
            {
                return BadRequest(new { error = "User not found" });
            }

            var chat = await chatService.CreateOrGetChatAsync(userId, request.UserId, ct);
        
            var participant = chat.UserId1 == userId ? chat.User2 : chat.User1;
        
            return Ok(new
            {
                id = chat.Id,
                participant = new
                {
                    id = participant.Id,
                    name = participant.Name
                },
                lastMessage = chat.LastMessage,
                lastMessageAt = chat.LastMessageAt,
                unreadCount = chat.UnreadCount
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating chat"); // only there to find error , but logger should be in many edge places
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet("sessions/{id}/messages")]
    public async Task<IActionResult> GetMessages(Guid id, CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var chat = await chatService.GetChatWithMessagesAsync(id, userId, ct);
        if (chat == null)
            return NotFound();

        var result = chat.Messages.Select(m => new
        {
            id = m.Id,
            chatId = m.ChatId,
            content = m.Content,
            senderId = m.SenderId,
            senderName = m.Sender.Name,
            createdAt = m.CreatedAt,
            isOwn = m.SenderId == userId
        });

        return Ok(result);
    }

    [HttpPost("sessions/{id}/messages")]
    public async Task<IActionResult> SendMessage(Guid id, [FromBody] SendMessageRequest request, CancellationToken ct)
    {  
        if (request == null)
        {
            logger.LogWarning("SendMessage: request is null");
            return BadRequest(new { error = "Invalid request body" });
        }
        
        logger.LogInformation("SendMessage: ChatId={ChatId}, Question={Question}", id, request?.Question);

        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        try
        {
            var message = await chatService.SendMessageAsync(id, userId, request.Question, ct);
            
            return Ok(new
            {
                id = message.Id,
                chatId = message.ChatId,
                content = message.Content,
                senderId = message.SenderId,
                senderName = User.FindFirst(ClaimTypes.Name)?.Value ?? "User",
                createdAt = message.CreatedAt,
                isOwn = true
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPatch("sessions/{id}")]
    public IActionResult RenameChat(Guid id, [FromBody] RenameChatRequest request)
    {
        return Ok(); // i don't think this is needed just will return ok
    }

    [HttpDelete("sessions/{id}")]
    public async Task<IActionResult> DeleteChat(Guid id, CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var result = await chatService.DeleteChatAsync(id, userId, ct);
        return result ? Ok() : NotFound();
    }
}

public class CreateChatRequest
{
    public Guid UserId { get; set; }
}

public class SendMessageRequest
{
    public string Question { get; set; } = string.Empty;
    public int? TopK { get; set; }
    public string? SystemPrompt { get; set; }
}

public class RenameChatRequest
{
    public string Title { get; set; } = string.Empty;
}
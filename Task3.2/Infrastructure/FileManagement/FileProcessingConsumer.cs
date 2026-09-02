using Application.Abstractions;
using Application.Messages;
using Domain.Models;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.FileManagement;

public sealed class FileProcessingConsumer(
    IFileRepository fileRepository,
    ILogger<FileProcessingConsumer> logger) : IConsumer<FileProcessingRequested>
{
    
    private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

    public async Task Consume(ConsumeContext<FileProcessingRequested> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        using var scope = logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = msg.CorrelationId,
            ["FileId"] = msg.FileId
        });

        logger.LogInformation(
            "Received FileProcessingRequested. File={Filename}, StorageKey={StorageKey}",
            msg.Filename, msg.StorageKey);

        var file = await fileRepository.GetByIdAsync(msg.FileId, ct);

        if (file is null)
        {
            logger.LogWarning("File not found. Acknowledging message.");
            return;
        }

        if (file.OrganisationId != msg.OrganisationId)
        {
            logger.LogWarning("Organisation mismatch. Acknowledging.");
            return;
        }

        if (file.Status == FileStatus.Ready)
        {
            logger.LogInformation("Already processed (Ready). Skipping (idempotent).");
            return;
        }

        try
        {
            file.Status = FileStatus.Processing;
            file.ProcessingError = null;
            file.UpdatedAt = DateTime.UtcNow;
            await fileRepository.UpdateAsync(file, ct);

            var fullPath = Path.Combine(_uploadPath, msg.StorageKey);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Physical file not found: {fullPath}");
            }

            // Simulate real processing (replace later with OCR / parsing / indexing)
            await Task.Delay(1500, ct);
            

            file.Status = FileStatus.Ready;
            file.ProcessingError = null;
            file.UpdatedAt = DateTime.UtcNow;
            await fileRepository.UpdateAsync(file, ct);

            logger.LogInformation("File processed successfully → Ready");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Processing failed");

            file.Status = FileStatus.Failed;
            file.ProcessingError = ex.Message;
            file.UpdatedAt = DateTime.UtcNow;
            await fileRepository.UpdateAsync(file, ct);

            // Rethrow => MassTransit retry / DLQ will handle it
            throw;
        }
    }
}
using Application.Abstractions;
using Application.DTOs;
using Application.Exceptions;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.FileManagement;

public sealed class FileUploadService(FileValidationHelper fileValidationHelper, IFileRepository fileRepository, IDistributedCache cache)
{
    public async Task<(int StatusCode, FileResponseDto? File, string? Error)> HandleUploadAsync(
        HttpRequest request,
        Guid ownerId,
        Guid organisationId,
        CancellationToken ct = default)
    {
        var uploadResult = await fileValidationHelper.UploadAndValidateFileAsync(request, cache, ct);

        if (uploadResult.StatusCode != 200)
        {
            return (uploadResult.StatusCode, null, uploadResult.Error ?? uploadResult.StatusDescription);
        }


        if (uploadResult.IsDuplicate && uploadResult.Checksum != null)
        {
            var existing = await fileRepository.GetByHashAsync(uploadResult.Checksum, organisationId, ct);
            if (existing != null)
            {
                return (200, MapToDto(existing), null); 
            }

        }

        var now = DateTime.UtcNow;
        var entity = new FileModel
        {
            Id = Guid.NewGuid(),
            Filename = uploadResult.OriginalFileName!,
            Size = uploadResult.Size ?? 0,
            Status = FileStatus.Processing,
            ContentType = uploadResult.ContentType!,
            Checksum = uploadResult.Checksum!,
            StorageKey = uploadResult.FileName!,
            OrganisationId = organisationId,
            OwnerId = ownerId,
            Application = null,
            ProcessingError = null,
            CreatedAt = now,
            UpdatedAt = now
        };

        try
        {
            await fileRepository.AddAsync(entity, ct);
        }
        catch (DuplicateFileException)
        {
            var existing = await fileRepository.GetByHashAsync(entity.Checksum, organisationId, ct);
            if (existing != null)
            {
                return (200, MapToDto(existing), null);
            }
            return (500, null, "Upload conflict could not be resolved");
        }
        
        return (201, MapToDto(entity), null);
    }

    private static FileResponseDto MapToDto(FileModel entity) => new(
    entity.Id,
    entity.Filename,
    entity.Size,
    entity.Status.ToString().ToLowerInvariant(),
    entity.ContentType,
    entity.Checksum,
    entity.StorageKey,
    entity.OrganisationId,
    entity.OwnerId,
    entity.Application,
    entity.ProcessingError,
    entity.CreatedAt,
    entity.UpdatedAt);

    public sealed class DuplicateFileException(string checksum, Guid organisationId, Exception inner)
        : Exception($"A file with checksum {checksum} already exists for organisation {organisationId}", inner)
    {
        public string Checksum { get; } = checksum;
        public Guid OrganisationId { get; } = organisationId;
    }
}
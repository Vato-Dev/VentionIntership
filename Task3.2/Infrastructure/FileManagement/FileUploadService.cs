using Application.Abstractions;
using Application.DTOs;
using Application.Exceptions;
using Application.Messages;
using Domain.Models;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.FileManagement;

public sealed class FileUploadService(
    FileValidationHelper fileValidationHelper,
    IFileRepository fileRepository,
    IDistributedCache cache,
    IPublishEndpoint publishEndpoint) : IFileUploadService
{
    public async Task<(int StatusCode, FileResponseDto? File, string? Error)> HandleUploadAsync(
        HttpRequest request,
        Guid ownerId,
        Guid organisationId,
        CancellationToken ct = default)
    {
        var uploadResult = await fileValidationHelper.UploadAndValidateFileAsync(request, cache, ct);

        if (uploadResult.StatusCode != 200)
            return (uploadResult.StatusCode, null, uploadResult.Error ?? uploadResult.StatusDescription);

        if (uploadResult.IsDuplicate && uploadResult.Checksum != null)
        {
            var existing = await fileRepository.GetByHashAsync(uploadResult.Checksum, organisationId, ct);
            if (existing != null)
                return (200, MapToDto(existing), null);
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
                return (200, MapToDto(existing), null);

            return (409, null, "Upload conflict could not be resolved");
        }

        await publishEndpoint.Publish(new FileProcessingRequested
        {
            FileId = entity.Id,
            OrganisationId = entity.OrganisationId,
            StorageKey = entity.StorageKey,
            ContentType = entity.ContentType,
            Filename = entity.Filename,
            CorrelationId = Guid.NewGuid()
        }, ct);

        return (201, MapToDto(entity), null);
    }

    public async Task<PagedResponse<FileResponseDto, Guid>> GetFilesByOrganizationAsync(
        Guid organisationId, int page, int pageSize, CancellationToken ct)
    {
        var paged = await fileRepository.GetByOrganizationIdAsync(organisationId, page, pageSize, ct);
        return new PagedResponse<FileResponseDto, Guid>
        {
            Data = paged.Data.Select(MapToDto).ToList(),
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize,
            TotalItems = paged.TotalItems,
            TotalPages = paged.TotalPages
        };
    }

    public async Task<(int StatusCode, string? Error)> DeleteFileAsync(
        Guid id, Guid organisationId, CancellationToken ct)
    {
        var file = await fileRepository.GetByIdAsync(id, ct);
        if (file is null) return (404, "File not found");
        if (file.OrganisationId != organisationId) return (403, "File does not belong to this organisation");

        await fileRepository.DeleteAsync(file, ct);
        return (204, null);
    }

    public async Task<(int StatusCode, string? Message, string? Error)> ReprocessFileAsync(
        Guid id, Guid organisationId, CancellationToken ct)
    {
        var file = await fileRepository.GetByIdAsync(id, ct);
        if (file is null) return (404, null, "File not found");
        if (file.OrganisationId != organisationId) return (403, null, "File does not belong to this organisation");

        file.Status = FileStatus.Processing;
        file.ProcessingError = null;
        file.UpdatedAt = DateTime.UtcNow;
        await fileRepository.UpdateAsync(file, ct);

        await publishEndpoint.Publish(new FileProcessingRequested
        {
            FileId = file.Id,
            OrganisationId = file.OrganisationId,
            StorageKey = file.StorageKey,
            ContentType = file.ContentType,
            Filename = file.Filename,
            CorrelationId = Guid.NewGuid()
        }, ct);

        return (200, "File reprocessing has been queued successfully", null);
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
}

public sealed class DuplicateFileException(string checksum, Guid organisationId, Exception inner)
    : Exception($"A file with checksum {checksum} already exists for organisation {organisationId}", inner)
{
    public string Checksum { get; } = checksum;
    public Guid OrganisationId { get; } = organisationId;
}
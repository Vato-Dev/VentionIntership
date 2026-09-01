using Application.Abstractions;
using Application.DTOs;
using Application.Exceptions;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.FileManagement;

public sealed class FileUploadService(
    FileValidationHelper fileValidationHelper, 
    IFileRepository fileRepository, 
    IDistributedCache cache) : IFileUploadService
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
            return (409, null, "Upload conflict could not be resolved");
        }
        
        return (201, MapToDto(entity), null);
    }

    public async Task<PagedResponse<FileResponseDto, Guid>> GetFilesByOrganizationAsync(
        Guid organisationId, 
        int page, 
        int pageSize, 
        CancellationToken ct)
    {
        var pagedFiles = await fileRepository.GetByOrganizationIdAsync(organisationId, page, pageSize, ct);

        var dtoData = pagedFiles.Data.Select(MapToDto).ToList();

        return new PagedResponse<FileResponseDto, Guid>
        {
            Data = dtoData,
            PageNumber = pagedFiles.PageNumber,
            PageSize = pagedFiles.PageSize,
            TotalItems = pagedFiles.TotalItems,
            TotalPages = pagedFiles.TotalPages
        };
    }

    public async Task<(int StatusCode, string? Error)> DeleteFileAsync(
        Guid id, 
        Guid organisationId, 
        CancellationToken ct)
    {
        var file = await fileRepository.GetByIdAsync(id, ct);
        if (file == null)
            return (404, "File not found");

        if (file.OrganisationId != organisationId)
            return (403, "File does not belong to this organisation");

        await fileRepository.DeleteAsync(file, ct);
        return (204, null); 
    }

    public async Task<(int StatusCode, string? Message, string? Error)> ReprocessFileAsync(
        Guid id, 
        Guid organisationId, 
        CancellationToken ct)
    {
        var file = await fileRepository.GetByIdAsync(id, ct);
        if (file == null)
            return (404, null, "File not found");

        if (file.OrganisationId != organisationId)
            return (403, null, "File does not belong to this organisation");

        file.Status = FileStatus.Processing;
        file.ProcessingError = null;
        file.UpdatedAt = DateTime.UtcNow;

        // TODO: when i'll add rabbit do smth like this
        // await _publishEndpoint.Publish(new FileReprocessCommand(file.Id));

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
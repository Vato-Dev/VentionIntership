using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Net.Http.Headers;

namespace Infrastructure.FileManagement;

public sealed class FileValidationHelper
{
    private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");


    private const int StreamBufferSize = 81920;
    private const int MagicByteReadSize = 32;

    // TODO: move this consts out probably in options
    private const long MaxFileSizeBytes = 100 * 1024 * 1024;

    private const string HashCacheKeyPrefix = "file-hash:";


    private static readonly HashSet<string> ZipBasedOfficeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.openxmlformats-officedocument.presentationml.presentation"
    };

    public async Task<FileUploadResult> UploadAndValidateFileAsync(HttpRequest request, IDistributedCache cache, CancellationToken ct = default) //todo: just realised that this helper does more than it should. 
    {
        var boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(request.ContentType).Boundary).Value;
        var reader = new MultipartReader(boundary, request.Body);
        MultipartSection? section;

        while ((section = await reader.ReadNextSectionAsync(ct)) != null)
        {
            var contentDisposition = section.GetContentDispositionHeader();

            if (contentDisposition != null && contentDisposition.IsFileDisposition())
            {

                return await HandleFileSectionAsync(section, contentDisposition, cache, ct);
            }

            if (contentDisposition != null && contentDisposition.IsFormDisposition())
            {
                using var streamReader = new StreamReader(section.Body);
                await streamReader.ReadToEndAsync(ct);
            }
        }

        return new FileUploadResult(400, "No file found in request");
    }

    private async Task<FileUploadResult> HandleFileSectionAsync(
        MultipartSection section,
        ContentDispositionHeaderValue contentDisposition,
        IDistributedCache cache,
        CancellationToken ct)
    {
        var declaredContentType = section.ContentType;
        var newName = $"{Guid.NewGuid()}{Path.GetExtension(contentDisposition.FileName.Value)}";
        var path = Path.Combine(_uploadPath, newName);


        var fullUploadRoot = Path.GetFullPath(_uploadPath);
        var fullTargetPath = Path.GetFullPath(path);
        if (!fullTargetPath.StartsWith(fullUploadRoot + Path.DirectorySeparatorChar, StringComparison.Ordinal))
        {
            return new FileUploadResult(400, "Invalid file path");
        }

        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }

        var isUploadSuccessful = false;

        try
        {
            var peekBuffer = new byte[MagicByteReadSize];
            var peekBytesRead = await ReadExactAsync(section.Body, peekBuffer, ct);
            if (peekBytesRead == 0)
                return new FileUploadResult(400, "Empty file");

            string detectedContentType;
            try
            {
                detectedContentType = DetectContentType(peekBuffer[..peekBytesRead]);
            }
            catch (Exception)
            {
                return new FileUploadResult(415, "Unsupported or unrecognized file type");
            }

            var contentTypeMatches =
                string.Equals(detectedContentType, declaredContentType, StringComparison.OrdinalIgnoreCase)
                || (detectedContentType == "application/zip" && ZipBasedOfficeTypes.Contains(declaredContentType));

            if (!contentTypeMatches)
                return new FileUploadResult(415, $"Declared content type '{declaredContentType}' does not match file contents");
            

            using var sha256 = SHA256.Create();
            long totalBytesRead = peekBytesRead;

            await using (var output = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, StreamBufferSize))
            await using (var cryptoStream = new CryptoStream(output, sha256, CryptoStreamMode.Write))
            {
                await cryptoStream.WriteAsync(peekBuffer.AsMemory(0, peekBytesRead), ct);

                var copyBuffer = new byte[StreamBufferSize];
                int bytesRead;
                while ((bytesRead = await section.Body.ReadAsync(copyBuffer, ct)) > 0)
                {
                    totalBytesRead += bytesRead;
                    if (totalBytesRead > MaxFileSizeBytes)
                        throw new InvalidOperationException("File exceeds the maximum allowed size");
                    

                    await cryptoStream.WriteAsync(copyBuffer.AsMemory(0, bytesRead), ct);
                }

                await cryptoStream.FlushFinalBlockAsync(ct);
            }

            var fileHash = Convert.ToHexString(sha256.Hash!);
            var cacheKey = HashCacheKeyPrefix + fileHash;

            var existingFileName = await cache.GetStringAsync(cacheKey, ct);
            if (existingFileName != null)
            {
                isUploadSuccessful = true; 
                File.Delete(path);
                return new FileUploadResult(
                    200,
                    "Duplicate content, reused existing file",
                    FileName: existingFileName,
                    OriginalFileName: contentDisposition.FileName.Value,
                    ContentType: detectedContentType,
                    Checksum: fileHash,
                    IsDuplicate: true);
            }
            
            var cacheOptions = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            };
            await cache.SetStringAsync(cacheKey, newName, cacheOptions, ct);
            isUploadSuccessful = true;
            return new FileUploadResult(
                200,
                "File uploaded",
                FileName: newName,
                OriginalFileName: contentDisposition.FileName.Value,
                Size: totalBytesRead,
                ContentType: detectedContentType,
                Checksum: fileHash);
        }
        catch (Exception)
        {
            return new FileUploadResult(413, "Something went wrong"); //maybe 409 status code could be better
        }
        finally
        {
            if (!isUploadSuccessful && File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    private static async Task<int> ReadExactAsync(Stream stream, byte[] buffer, CancellationToken ct)
    {
        var totalRead = 0;
        while (totalRead < buffer.Length)
        {
            var read = await stream.ReadAsync(buffer.AsMemory(totalRead, buffer.Length - totalRead), ct);
            if (read == 0) break;
            totalRead += read;
        }
        return totalRead;
    }

    public Dictionary<string, string> AllowedContentTypeWithHexStrings => new()
    {
        { "image/jpeg", "FFD8FF" },
        { "image/png", "89504E470D0A1A0A" },
        { "image/gif", "47494638" },
        { "image/bmp", "424D" },
        { "image/tiff", "49492A00" },
        { "image/webp", "52494646" },
        { "application/pdf", "25504446" },
        { "application/zip", "504B0304" },
        { "application/x-rar-compressed", "526172211A07" },
        { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "504B0304" },
        { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "504B0304" },
        { "application/vnd.openxmlformats-officedocument.presentationml.presentation", "504B0304" },
        { "application/x-msdownload", "4D5A" },
        { "application/x-elf", "7F454C46" }
    };

    public string DetectContentType(byte[] fileBytes)
    {
        if (fileBytes == null || fileBytes.Length == 0)
            throw new Exception("Unknown content type or empty file");

        var bytesToRead = Math.Min(fileBytes.Length, 32);
        var fileHex = Convert.ToHexString(fileBytes, 0, bytesToRead);

        var matchedType = AllowedContentTypeWithHexStrings
            .OrderByDescending(x => x.Value.Length)
            .FirstOrDefault(x => fileHex.StartsWith(x.Value));

        return matchedType.Key ?? throw new Exception("Unknown content type or empty file");
    }
}

public sealed record FileUploadResult(
    int StatusCode,
    string StatusDescription,
    string? Error = null,
    string? FileName = null,
    string? OriginalFileName = null,
    long? Size = null,
    string? ContentType = null,
    string? Checksum = null,
    bool IsDuplicate = false);
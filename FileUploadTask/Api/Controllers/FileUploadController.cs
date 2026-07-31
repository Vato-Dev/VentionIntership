using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace Api.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public sealed class FileUploadController : ControllerBase
    {
        private static readonly string[] AllowedFormats = ["image/jpeg", "image/png", "image/webp", "image/Bmp", "image/pbm", "image/Tiff"];

        private static readonly string[] AllowedVideoFormats = ["video/mp4", "video/webm", "video/ogg", "video/quicktime", "video/x-msvideo", "video/mpeg"];
        private readonly string _uploadPath = $"{Directory.GetCurrentDirectory()}/uploads";
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(IFormFileCollection files)
        {
            if (files.Count == 0)
                return BadRequest("No files selected");
            List<string> names = new List<string>();
            foreach (var file in files)
            {
                var extension = Path.GetExtension(file.FileName);

                if (!IsFileSafe(file.FileName, file.ContentType))
                {
                    return new UnsupportedMediaTypeResult();
                }

                var newName = $"{Guid.NewGuid().ToString()}{extension}";
                if (!Directory.Exists(_uploadPath))
                    Directory.CreateDirectory(_uploadPath);

                var newPathForFile = Path.Combine(_uploadPath, newName);
                using var filestream = System.IO.File.Create(newPathForFile);
                await file.CopyToAsync(filestream, HttpContext.RequestAborted);
                names.Add(newName);
            }
            return Ok(names);
        }


        // I used documentation as example
        private const int BufferSize = 64 * 1024 * 1024; // 64 MB buffer size

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Route("/api/[controller]/uploadMultipart")]
        public async Task<IActionResult> Upload() //or add parameter Iformfile since swagger can't understand what to generate , but i'll use postman for tests
        {
            var boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(Request.ContentType).Boundary)
                .Value; //used AI here since if method receives any parameter swagger/postman automatically understands it as 'Content-Type: application/octet-stream' 

            var ct = HttpContext.RequestAborted;

            var reader = new MultipartReader(boundary, Request.Body);
            MultipartSection? section;
            long totalBytesRead = 0;

            while ((section = await reader.ReadNextSectionAsync(ct)) != null)
            {
                bool isUploadSuccessful = false;
                var contentdisposition = section.GetContentDispositionHeader();

                if (contentdisposition != null && contentdisposition.IsFileDisposition())
                {
                    var fileName = contentdisposition.FileName.Value;
                    var contentType = section.ContentType;
                    if (!IsFileSafe(fileName!, contentType!))
                    {
                        return new UnsupportedMediaTypeResult();
                    }
                    var newName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(contentdisposition.FileName.Value)}";
                    var path = Path.Combine(_uploadPath, newName);

                    try
                    {
                        if (!Directory.Exists(_uploadPath))
                        {
                            Directory.CreateDirectory(_uploadPath);
                        }

                        using var sha256 = System.Security.Cryptography.SHA256.Create();
                        string fileHash = string.Empty;

                        await using (FileStream output = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize: BufferSize))
                        {
                            await using (var cryptoStream = new System.Security.Cryptography.CryptoStream(output, sha256, System.Security.Cryptography.CryptoStreamMode.Write))
                            {
                                await section.Body.CopyToAsync(cryptoStream, ct);
            
                                await cryptoStream.FlushFinalBlockAsync(ct);
                            }
                        }

                        fileHash = Convert.ToHexString(sha256.Hash!); // i would save this hash in redis+ db, i think it's best solution ,but won't implement for now

                        totalBytesRead += section.Body.Length;
                        isUploadSuccessful = true;
                    }
                    catch (Exception)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "something went wrong");
                    }
                    finally
                    {
                        if (!isUploadSuccessful && System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }
                }
                else if (contentdisposition != null && contentdisposition.IsFormDisposition())
                {
                    using var streanReader = new StreamReader(section.Body);
                    await streanReader.ReadToEndAsync(ct);
                }
            }
            return Ok(totalBytesRead);
        }
        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp", ".bmp", ".mp4"];

        private bool IsFileSafe(string fileName, string contentType)
        {
            var extension = Path.GetExtension(fileName);
            bool isExtensionAllowed = AllowedExtensions.Any(x => x.Equals(extension, StringComparison.OrdinalIgnoreCase));

            bool isContentTypeAllowed = AllowedFormats.Any(x => x.Equals(contentType, StringComparison.OrdinalIgnoreCase)) || AllowedVideoFormats.Any(x => x.Equals(contentType, StringComparison.OrdinalIgnoreCase));

            return isExtensionAllowed && isContentTypeAllowed;
        }
    }
}

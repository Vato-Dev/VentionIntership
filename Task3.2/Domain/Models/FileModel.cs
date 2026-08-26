namespace Domain.Models
{
    public sealed class FileModel : DomainEntity<Guid>
    {
        public string Filename { get; set; } = default!;
        public long Size { get; set; }
        public FileStatus Status { get; set; }
        public string ContentType { get; set; } = default!;
        public string Checksum { get; set; } = default!;
        public string StorageKey { get; set; } = default!;
        public Guid OrganisationId { get; set; }
        public Guid OwnerId { get; set; } 
        public string? Application { get; set; }
        public string? ProcessingError { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public enum FileStatus
    {
        Processing,
        Processed,
        Error
    }

}

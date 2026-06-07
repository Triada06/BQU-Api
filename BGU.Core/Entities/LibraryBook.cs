namespace BGU.Core.Entities;

public class LibraryBook : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string[] Authors { get; set; } = [];
    public string? Description { get; set; }
    public string? Isbn { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string? Publisher { get; set; }
    public int? PublishedYear { get; set; }
    public string? Edition { get; set; }
    public string[] Tags { get; set; } = [];
    public string Format { get; set; } = "other";
    public string Status { get; set; } = "draft";
    public string? CoverImageFileName { get; set; }
    public string? StoredFileName { get; set; }
    public string? FileName { get; set; }
    public long? FileSizeBytes { get; set; }
    public string? FileContentType { get; set; }
    public int ViewCount { get; set; }
    public string? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

using Microsoft.AspNetCore.Http;

namespace BGU.Application.Contracts.Library.Requests;

public class UpsertLibraryBookRequest
{
    public string Title { get; set; } = string.Empty;
    public List<string> Authors { get; set; } = [];
    public string? Description { get; set; }
    public string? Isbn { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string? Publisher { get; set; }
    public int? PublishedYear { get; set; }
    public string? Edition { get; set; }
    public List<string> Tags { get; set; } = [];
    public string Status { get; set; } = "draft";
    public IFormFile? BookFile { get; set; }
    public IFormFile? CoverImage { get; set; }
}

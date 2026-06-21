using BGU.Application.Common;
using BGU.Application.Contracts.Library.Requests;
using BGU.Application.Dtos.Library;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BGU.Application.Services;

public class LibraryService(
    AppDbContext context,
    ILibraryBookRepository libraryBookRepository,
    IWebHostEnvironment env,
    IHttpContextAccessor httpContextAccessor) : ILibraryService
{
    private const string DefaultContentType = "application/octet-stream";

    public async Task<ApiResult<PagedResponse<LibraryBookDto>>> GetAllAsync(
        string? query,
        string? category,
        string? status,
        int page,
        int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var booksQuery = context.LibraryBooks
            .AsNoTracking()
            .Include(x => x.CreatedBy)
            .AsQueryable();

        var cleanQuery = query?.Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(cleanQuery))
        {
            booksQuery = booksQuery.Where(x =>
                x.Title.ToLower().Contains(cleanQuery) ||
                x.Category.ToLower().Contains(cleanQuery) ||
                (x.Description != null && x.Description.ToLower().Contains(cleanQuery)) ||
                (x.Isbn != null && x.Isbn.ToLower().Contains(cleanQuery)) ||
                (x.Publisher != null && x.Publisher.ToLower().Contains(cleanQuery)) ||
                x.Authors.Any(a => a.ToLower().Contains(cleanQuery)) ||
                x.Tags.Any(t => t.ToLower().Contains(cleanQuery)));
        }

        var cleanCategory = category?.Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(cleanCategory))
        {
            booksQuery = booksQuery.Where(x => x.Category.ToLower() == cleanCategory);
        }

        var cleanStatus = status?.Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(cleanStatus))
        {
            booksQuery = booksQuery.Where(x => x.Status == cleanStatus);
        }

        var totalCount = await booksQuery.CountAsync();
        var books = await booksQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return ApiResult<PagedResponse<LibraryBookDto>>.Success(new PagedResponse<LibraryBookDto>
        {
            Items = books.Select(MapToDto),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }

    public async Task<ApiResult<LibraryBookDto>> GetByIdAsync(string id)
    {
        var book = await context.LibraryBooks
            .AsNoTracking()
            .Include(x => x.CreatedBy)
            .FirstOrDefaultAsync(x => x.Id == id);

        return book is null
            ? ApiResult<LibraryBookDto>.NotFound("Book not found")
            : ApiResult<LibraryBookDto>.Success(MapToDto(book));
    }

    public async Task<ApiResult<LibraryBookDto>> CreateAsync(UpsertLibraryBookRequest request, string? userId)
    {
        var savedBookFile = await SaveFileAsync(request.BookFile, GetBooksPath());
        var savedCoverFile = await SaveFileAsync(request.CoverImage, GetCoversPath());

        var book = new LibraryBook
        {
            Title = request.Title.Trim(),
            Authors = CleanValues(request.Authors),
            Description = CleanOptional(request.Description),
            Isbn = CleanOptional(request.Isbn),
            Category = request.Category.Trim(),
            Language = request.Language.Trim(),
            Publisher = CleanOptional(request.Publisher),
            PublishedYear = request.PublishedYear,
            Edition = CleanOptional(request.Edition),
            Tags = CleanValues(request.Tags),
            Format = GetFormat(request.BookFile?.FileName),
            Status = request.Status.Trim().ToLowerInvariant(),
            StoredFileName = savedBookFile?.StoredFileName,
            FileName = savedBookFile?.OriginalFileName,
            FileSizeBytes = request.BookFile?.Length,
            FileContentType = GetUploadContentType(request.BookFile),
            CoverImageFileName = savedCoverFile?.StoredFileName,
            CreatedById = userId
        };

        if (await libraryBookRepository.CreateAsync(book))
        {
            return await GetByIdAsync(book.Id);
        }

        DeleteFileIfExists(GetBooksPath(), savedBookFile?.StoredFileName);
        DeleteFileIfExists(GetCoversPath(), savedCoverFile?.StoredFileName);
        return ApiResult<LibraryBookDto>.SystemError("Failed to create library book");
    }

    public async Task<ApiResult<LibraryBookDto>> UpdateAsync(string id, UpsertLibraryBookRequest request)
    {
        var book = await libraryBookRepository.GetByIdAsync(id, tracking: true);
        if (book is null)
        {
            return ApiResult<LibraryBookDto>.NotFound("Book not found");
        }

        var oldBookFile = book.StoredFileName;
        var oldCoverFile = book.CoverImageFileName;
        var savedBookFile = await SaveFileAsync(request.BookFile, GetBooksPath());
        var savedCoverFile = await SaveFileAsync(request.CoverImage, GetCoversPath());

        book.Title = request.Title.Trim();
        book.Authors = CleanValues(request.Authors);
        book.Description = CleanOptional(request.Description);
        book.Isbn = CleanOptional(request.Isbn);
        book.Category = request.Category.Trim();
        book.Language = request.Language.Trim();
        book.Publisher = CleanOptional(request.Publisher);
        book.PublishedYear = request.PublishedYear;
        book.Edition = CleanOptional(request.Edition);
        book.Tags = CleanValues(request.Tags);
        book.Status = request.Status.Trim().ToLowerInvariant();
        book.UpdatedAt = DateTime.UtcNow;

        if (savedBookFile is not null)
        {
            book.StoredFileName = savedBookFile.StoredFileName;
            book.FileName = savedBookFile.OriginalFileName;
            book.FileSizeBytes = request.BookFile?.Length;
            book.FileContentType = GetUploadContentType(request.BookFile);
            book.Format = GetFormat(request.BookFile?.FileName);
        }

        if (savedCoverFile is not null)
        {
            book.CoverImageFileName = savedCoverFile.StoredFileName;
        }

        if (await libraryBookRepository.UpdateAsync(book))
        {
            if (savedBookFile is not null)
            {
                DeleteFileIfExists(GetBooksPath(), oldBookFile);
            }

            if (savedCoverFile is not null)
            {
                DeleteFileIfExists(GetCoversPath(), oldCoverFile);
            }

            return await GetByIdAsync(book.Id);
        }

        DeleteFileIfExists(GetBooksPath(), savedBookFile?.StoredFileName);
        DeleteFileIfExists(GetCoversPath(), savedCoverFile?.StoredFileName);
        return ApiResult<LibraryBookDto>.SystemError("Failed to update library book");
    }

    public async Task<ApiResult> DeleteAsync(string id)
    {
        var book = await libraryBookRepository.GetByIdAsync(id, tracking: true);
        if (book is null)
        {
            return ApiResult.NotFound("Book not found");
        }

        var storedFileName = book.StoredFileName;
        var coverFileName = book.CoverImageFileName;

        if (!await libraryBookRepository.DeleteAsync(book))
        {
            return ApiResult.SystemError("Failed to delete library book");
        }

        DeleteFileIfExists(GetBooksPath(), storedFileName);
        DeleteFileIfExists(GetCoversPath(), coverFileName);

        return ApiResult.Success();
    }

    public async Task<ApiResult<LibraryBookFileDto>> GetCoverAsync(string id)
    {
        var book = await libraryBookRepository.GetByIdAsync(id, tracking: false);
        if (book is null)
        {
            return ApiResult<LibraryBookFileDto>.NotFound("Book not found");
        }

        var coverResult = ResolveCoverFile(book);
        return coverResult is null
            ? ApiResult<LibraryBookFileDto>.NotFound("Book cover not found")
            : ApiResult<LibraryBookFileDto>.Success(coverResult);
    }

    public async Task<ApiResult<LibraryBookFileDto>> GetReadFileAsync(string id)
    {
        var book = await libraryBookRepository.GetByIdAsync(id, tracking: true);
        if (book is null)
        {
            return ApiResult<LibraryBookFileDto>.NotFound("Book not found");
        }

        var fileResult = ResolveBookFile(book);
        if (fileResult is null)
        {
            return ApiResult<LibraryBookFileDto>.NotFound("Book file not found");
        }

        book.ViewCount++;
        await libraryBookRepository.UpdateAsync(book);

        return ApiResult<LibraryBookFileDto>.Success(fileResult);
    }



    private LibraryBookFileDto? ResolveBookFile(LibraryBook book)
    {
        if (string.IsNullOrWhiteSpace(book.StoredFileName))
        {
            return null;
        }

        var fullPath = Path.Combine(GetBooksPath(), book.StoredFileName);
        if (!File.Exists(fullPath))
        {
            return null;
        }

        return new LibraryBookFileDto(
            fullPath,
            GetBookContentType(book),
            book.FileName ?? book.StoredFileName);
    }

    private LibraryBookFileDto? ResolveCoverFile(LibraryBook book)
    {
        if (string.IsNullOrWhiteSpace(book.CoverImageFileName))
        {
            return null;
        }

        var fullPath = Path.Combine(GetCoversPath(), book.CoverImageFileName);
        if (!File.Exists(fullPath))
        {
            return null;
        }

        return new LibraryBookFileDto(
            fullPath,
            GetContentType(book.CoverImageFileName),
            book.CoverImageFileName);
    }

    private LibraryBookDto MapToDto(LibraryBook book)
    {
        var createdByFullName = book.CreatedBy is null
            ? null
            : $"{book.CreatedBy.Name} {book.CreatedBy.Surname} {book.CreatedBy.MiddleName}".Trim();

        return new LibraryBookDto(
            book.Id,
            book.Title,
            book.Authors,
            book.Description,
            book.Isbn,
            book.Category,
            book.Language,
            book.Publisher,
            book.PublishedYear,
            book.Edition,
            book.Tags,
            book.Format,
            book.Status,
            BuildCoverUrl(book.Id, book.CoverImageFileName),
            book.FileName,
            book.FileSizeBytes,
            book.FileContentType,
            BuildReadUrl(book.Id, book.StoredFileName),
            book.ViewCount,
            book.CreatedById,
            createdByFullName,
            book.CreatedAt.ToString("O"),
            book.UpdatedAt?.ToString("O"));
    }

    private async Task<SavedFile?> SaveFileAsync(IFormFile? file, string folder)
    {
        if (file is null || file.Length == 0)
        {
            return null;
        }

        Directory.CreateDirectory(folder);
        var originalFileName = Path.GetFileName(file.FileName);
        var storedFileName = $"{Guid.NewGuid()}-{SanitizeFileName(originalFileName)}";
        var fullPath = Path.Combine(folder, storedFileName);

        await using var writer = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(writer);

        return new SavedFile(originalFileName, storedFileName);
    }

    private string GetBooksPath()
        => Path.Combine(GetLibraryPath(), "Files");

    private string GetCoversPath()
        => Path.Combine(GetLibraryPath(), "Covers");

    private string GetLibraryPath()
    {
        var webRoot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
        var path = Path.Combine(webRoot, "Library", "Books");
        Directory.CreateDirectory(path);
        return path;
    }

    private string? BuildCoverUrl(string id, string? fileName)
        => string.IsNullOrWhiteSpace(fileName)
            ? null
            : BuildPublicUrl($"/api/library/books/{Uri.EscapeDataString(id)}/cover");

    private string? BuildReadUrl(string id, string? fileName)
        => string.IsNullOrWhiteSpace(fileName)
            ? null
            : BuildPublicUrl($"/api/library/books/{Uri.EscapeDataString(id)}/read");

    private string BuildPublicUrl(string relativePath)
    {
        var request = httpContextAccessor.HttpContext?.Request;
        if (request is null)
        {
            return relativePath;
        }

        return $"{request.Scheme}://{request.Host}{request.PathBase}{relativePath}";
    }

    private static string[] CleanValues(IEnumerable<string>? values)
        => values?
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray() ?? [];

    private static string? CleanOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? GetUploadContentType(IFormFile? file)
    {
        if (file is null)
        {
            return null;
        }

        var contentTypeFromFileName = GetContentType(file.FileName);
        if (!string.Equals(contentTypeFromFileName, DefaultContentType, StringComparison.OrdinalIgnoreCase))
        {
            return contentTypeFromFileName;
        }

        return string.IsNullOrWhiteSpace(file.ContentType) ? DefaultContentType : file.ContentType;
    }

    private static string GetBookContentType(LibraryBook book)
    {
        var contentTypeFromFileName = GetContentType(book.FileName ?? book.StoredFileName ?? string.Empty);
        if (!string.Equals(contentTypeFromFileName, DefaultContentType, StringComparison.OrdinalIgnoreCase))
        {
            return contentTypeFromFileName;
        }

        return string.IsNullOrWhiteSpace(book.FileContentType) ? DefaultContentType : book.FileContentType;
    }

    private static string GetFormat(string? fileName)
    {
        var extension = Path.GetExtension(fileName ?? string.Empty).TrimStart('.').ToLowerInvariant();
        return extension is "pdf"
            ? extension
            : "other";
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => DefaultContentType
        };
    }

    private static string SanitizeFileName(string fileName)
    {
        foreach (var invalidChar in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(invalidChar, '_');
        }

        return fileName;
    }

    private static void DeleteFileIfExists(string folder, string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return;
        }

        var path = Path.Combine(folder, fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private sealed record SavedFile(string OriginalFileName, string StoredFileName);
}

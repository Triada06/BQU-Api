using BGU.Application.Common;
using BGU.Application.Contracts.Library.Requests;
using BGU.Application.Dtos.Library;
using BGU.Infrastructure.Constants;

namespace BGU.Application.Services.Interfaces;

public interface ILibraryService
{
    Task<ApiResult<PagedResponse<LibraryBookDto>>> GetAllAsync(
        string? query,
        string? category,
        string? status,
        int page,
        int pageSize);

    Task<ApiResult<LibraryBookDto>> GetByIdAsync(string id);
    Task<ApiResult<LibraryBookDto>> CreateAsync(UpsertLibraryBookRequest request, string? userId);
    Task<ApiResult<LibraryBookDto>> UpdateAsync(string id, UpsertLibraryBookRequest request);
    Task<ApiResult> DeleteAsync(string id);
    Task<ApiResult<LibraryBookFileDto>> GetCoverAsync(string id);
    Task<ApiResult<LibraryBookFileDto>> GetReadFileAsync(string id);
}

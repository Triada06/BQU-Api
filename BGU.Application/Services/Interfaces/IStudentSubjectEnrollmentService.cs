using BGU.Application.Common;
using BGU.Application.Dtos.StudentEnrollment;
using BGU.Infrastructure.Constants;

namespace BGU.Application.Services.Interfaces;

public interface IStudentSubjectEnrollmentService
{
    Task<ApiResult<string>> CreateAsync(CreateStudentSubjectEnrollmentDto dto);
    Task<ApiResult<PagedResponse<GetEnrollmentDto>>> GetAllAsync(int page, int pageSize);
    Task<ApiResult<GetEnrollmentDto>> GetByIdAsync(string id);
    Task<ApiResult> UpdateAsync(string id, UpdateStudentSubjectEnrollmentDto dto);
    Task<ApiResult> DeleteAsync(string id);
}
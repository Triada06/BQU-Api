using BGU.Application.Common;
using BGU.Application.Dtos.Exams;
using BGU.Infrastructure.Constants;

namespace BGU.Application.Services.Interfaces;

public interface IFinalService
{
    Task<ApiResult<PagedResponse<GetFinalDto>>> GetAllAsync(int page, int pageSize, string? search);
    Task<ApiResult<bool>> SetExamDateAsync(SetExamDto setExamDto);
    Task<ApiResult<string>> CreateAsync(CreateExamDto createExamDto);
    Task<ApiResult<UpdateExamResponse>> UpdateAsync(UpdateExamRequest request);
    Task<ApiResult<ExamsToGrade>> GetAllByTeachAsync(string userId);
    Task<ApiResult<string>> GradeAsync(string userId, string finalId, int grade);
    Task<ApiResult<bool>> ConfirmAsync(string finalId);
    Task<ApiResult<bool>> SetGroupExamDateAsync(SetGroupExamDto setExamDto);
}
using BGU.Application.Common;
using BGU.Application.Dtos.Exams;
using BGU.Infrastructure.Constants;

namespace BGU.Application.Services.Interfaces;

public interface IFinalService
{
    Task<ApiResult<PagedResponse<GetFinalDto>>> GetAllAsync(int page, int pageSize, string? search,  string? groupId); // todo : wrap in dto
    Task<ApiResult<IEnumerable<GetFinalDto>>> GetAllToConfirmAsync();
    Task<ApiResult> SetExamDateAsync(SetExamDto setExamDto);
    Task<ApiResult<string>> CreateAsync(CreateExamDto createExamDto);
    Task<ApiResult<UpdateExamResponse>> UpdateAsync(UpdateExamRequest request);
    Task<ApiResult<ExamsToGrade>> GetAllByTeachAsync(string userId, bool forGrade);
    Task<ApiResult<string>> GradeAsync(string userId, string finalId, int grade);
    Task<ApiResult> ConfirmAsync(string finalId);
    Task<ApiResult> BulkConfirmAsync(BulkConfirmFinalsRequest? request);
    Task<ApiResult> SetGroupExamDateAsync(SetGroupExamDto setExamDto);
}

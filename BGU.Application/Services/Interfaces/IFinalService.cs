using BGU.Application.Common;
using BGU.Application.Dtos.Exams;

namespace BGU.Application.Services.Interfaces;

public interface IFinalService
{
    Task<ApiResult<GetAllFinalDto>> GetAllAsync();
    Task<bool> SetExamDateAsync(SetExamDto setExamDto);
    Task<ApiResult<string>> CreateAsync(CreateExamDto createExamDto);
    Task<ApiResult<UpdateExamResponse>> UpdateAsync(UpdateExamDto updateExamDto);
}
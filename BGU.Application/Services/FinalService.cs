using BGU.Application.Common;
using BGU.Application.Dtos.Exams;
using BGU.Application.Services.Interfaces;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Application.Services;

public class FinalService(IFinalRepository finalRepository) : IFinalService 
{
    public Task<ApiResult<GetAllFinalDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> SetExamDateAsync(SetExamDto setExamDto)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResult<string>> CreateAsync(CreateExamDto createExamDto)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResult<UpdateExamResponse>> UpdateAsync(UpdateExamDto updateExamDto)
    {
        throw new NotImplementedException();
    }
}
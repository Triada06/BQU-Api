using BGU.Application.Common;
using BGU.Application.Contracts.IndependentWorks.Requests;
using BGU.Application.Contracts.IndependentWorks.Responses;
using BGU.Application.Dtos.IndependentWorks;

namespace BGU.Application.Services.Interfaces;

public interface IIndependentWorkService
{
    Task<CreateIndependentWorkResponse> CreateAsync(GradeIndependentWorkRequest  request);
    Task<DeleteIndependentWorkResponse> DeleteAsync(string id);
    Task<ApiResult<GradeIndependentWorkDto>> BulkGradeIndependentWorkAsync(string id,GradeIndependentWorkDto dto);
}
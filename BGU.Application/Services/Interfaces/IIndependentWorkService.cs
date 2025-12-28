using BGU.Application.Contracts;
using BGU.Application.Contracts.IndependentWorks.Requests;
using BGU.Application.Contracts.IndependentWorks.Responses;

namespace BGU.Application.Services.Interfaces;

public interface IIndependentWorkService
{
    Task<CreateIndependentWorkResponse> CreateAsync(GradeIndependentWorkRequest  request);
    Task<DeleteIndependentWorkResponse> DeleteAsync(string id);
}
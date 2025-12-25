using BGU.Application.Contracts.Colloquium.Requests;
using BGU.Application.Contracts.Colloquium.Responses;

namespace BGU.Application.Services.Interfaces;

public interface IColloquiumService
{
    Task<CreateColloquiumResponse> CreateAsync(CreateColloquiumRequest request);
    Task<DeleteColloquiumResponse> DeleteAsync(string id);
    Task<GetAllColloquiumResponse> GetAllAsync(string taughtSubjectId);
}
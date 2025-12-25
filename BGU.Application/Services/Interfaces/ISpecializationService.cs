using BGU.Application.Contracts.Specializations;

namespace BGU.Application.Services.Interfaces;

public interface ISpecializationService
{
    Task<GetAllSpecializationsResponse> GetAllAsync(int page, int pageSize, bool tracking = false);
}
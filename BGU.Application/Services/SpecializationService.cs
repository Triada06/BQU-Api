using BGU.Application.Contracts.Specializations;
using BGU.Application.Dtos.Specialization;
using BGU.Application.Services.Interfaces;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Application.Services;

public class SpecializationService(ISpecializationRepository specializationRepository) : ISpecializationService
{
    public async Task<GetAllSpecializationsResponse> GetAllAsync(int page, int pageSize, bool tracking = false)
    {
        var specializations = (await specializationRepository.GetAllAsync(page, pageSize, tracking)).ToList();
        return new GetAllSpecializationsResponse(specializations.Count != 0
            ? specializations.Select(x => new GetSpecializationDto(x.Id, x.Name, x.FacultyId))
            : [], StatusCode.Ok, true, ResponseMessages.Success);
    }
}
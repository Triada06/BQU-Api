using BGU.Application.Dtos.Specialization;
using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Specializations;

public sealed record GetAllSpecializationsResponse(
    IEnumerable<SpecializationDto> Specializations,
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);
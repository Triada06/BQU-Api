using BGU.Core.Enums;

namespace BGU.Application.Dtos.IndependentWorks;

public sealed record GetIndependentWorkDto(string Id, int Number, Grade? Grade);

public sealed record GetIndependentWorksDto(List<GetIndependentWorkDto> GetIndependentWorkDto);
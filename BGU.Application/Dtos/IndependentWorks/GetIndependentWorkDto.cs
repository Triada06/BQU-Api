namespace BGU.Application.Dtos.IndependentWorks;

public sealed record GetIndependentWorkDto(string Id, int Number, bool? IsPassed);

public sealed record GetIndependentWorksDto(List<GetIndependentWorkDto> GetIndependentWorkDto);
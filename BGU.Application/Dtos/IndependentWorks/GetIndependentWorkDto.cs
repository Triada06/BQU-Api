namespace BGU.Application.Dtos.IndependentWorks;

public sealed record GetIndependentWorkDto(string Id, DateTime Date, bool IsPassed);

public sealed record GetIndependentWorksDto(List<GetIndependentWorkDto> GetIndependentWorkDto);
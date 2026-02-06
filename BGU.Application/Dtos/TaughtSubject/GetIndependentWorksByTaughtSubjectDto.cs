namespace BGU.Application.Dtos.TaughtSubject;

public sealed record GetIndependentWorksByTaughtSubjectDto(List<GetIndependentWorkByTaughtSubjectDto> IndependentWorks);

public sealed record GetIndependentWorkByTaughtSubjectDto(string Id, string StudentId, int Number, bool? IsPassed);
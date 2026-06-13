using FluentValidation;

namespace BGU.Application.Dtos.Exams;

public sealed record SetExamDto(string Id, DateTime Date);

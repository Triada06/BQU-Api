using FluentValidation;

namespace BGU.Application.Contracts.IndependentWorks.Requests;

public sealed record BulkGradeIndependentWorkRequest(string IndependentWorkId, bool? IsPassed);
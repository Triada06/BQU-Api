using BGU.Core.Enums;

namespace BGU.Application.Contracts.Seminars.Requests;

public sealed record GradeSeminarAndClassRequest(Grade Grade, string ClassId, string StudentId);
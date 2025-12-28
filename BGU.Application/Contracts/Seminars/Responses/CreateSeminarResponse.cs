using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Seminars.Responses;

public sealed record CreateSeminarResponse(string? Id , StatusCode StatusCode, bool IsSucceeded, string ResponserMessage);
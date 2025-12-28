using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Seminars.Responses;

public sealed record DeleteSeminarResponse(StatusCode StatusCode, bool IsSucceeded, string ResponserMessage);
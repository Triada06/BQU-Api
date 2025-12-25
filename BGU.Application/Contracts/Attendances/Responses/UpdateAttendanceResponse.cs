using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Attendances.Responses;

public sealed record UpdateAttendanceResponse(string? Id,StatusCode StatusCode, bool IsSucceeded, string ResponseMessage);
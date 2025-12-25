using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Class.Responses;

public sealed record CreateClassResponse(string ClassId, StatusCode StatusCode, bool IsSucceeded, string ResponseMessage);
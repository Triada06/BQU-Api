using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Colloquium.Responses;

public sealed record DeleteColloquiumResponse(StatusCode StatusCode, bool IsSucceeded, string ResponseMessage);
using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Colloquium.Responses;

public sealed record CreateColloquiumResponse(
    string? ColloquiumId,
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);
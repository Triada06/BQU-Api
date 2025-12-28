using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.IndependentWorks.Responses;

public sealed record CreateIndependentWorkResponse(
    string? Id,
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);    
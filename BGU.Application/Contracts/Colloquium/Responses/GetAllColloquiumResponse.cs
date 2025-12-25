using BGU.Application.Dtos.Colloquiums;
using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Colloquium.Responses;

public sealed record GetAllColloquiumResponse(
    IEnumerable<ColloquiumDto> Colloquiums,
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);
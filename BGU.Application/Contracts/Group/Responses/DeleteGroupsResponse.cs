using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Group.Responses;

public record DeleteGroupsResponse(
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);
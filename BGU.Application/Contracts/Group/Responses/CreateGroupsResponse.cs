using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Group.Responses;

public record CreateGroupsResponse(string? GroupId,
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);
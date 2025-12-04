using BGU.Application.Dtos.Group;
using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Group.Responses;

public record GetByIdGroupsResponse(
    GetGroupDto? GroupDto,
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);
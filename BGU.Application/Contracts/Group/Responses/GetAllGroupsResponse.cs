using BGU.Application.Dtos.Group;
using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Group.Responses;

public record GetAllGroupsResponse(
    IEnumerable<GetGroupDto>? GroupsDto,
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);
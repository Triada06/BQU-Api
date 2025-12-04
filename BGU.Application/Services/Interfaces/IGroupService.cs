using BGU.Application.Contracts.Group;
using BGU.Application.Contracts.Group.Requests;
using BGU.Application.Contracts.Group.Responses;

namespace BGU.Application.Services.Interfaces;

public interface IGroupService
{
    Task<GetAllGroupsResponse> GetAllAsync(int page, int pageSize, bool tracking = false);
    Task<GetByIdGroupsResponse> GetByIdAsync(string id,  bool tracking = false);
    Task<DeleteGroupsResponse> DeleteAsync(string id);
    Task<UpdateGroupsResponse> UpdateAsync(string id, UpdateGroupRequest request);
}
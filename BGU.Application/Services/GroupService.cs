using BGU.Application.Contracts.Group;
using BGU.Application.Contracts.Group.Requests;
using BGU.Application.Contracts.Group.Responses;
using BGU.Application.Services.Interfaces;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Application.Services;

public class GroupService(IGroupRepository groupRepository) : IGroupService
{
    public Task<GetAllGroupsResponse> GetAllAsync(int page, int pageSize, bool tracking = false)
    {
        throw new NotImplementedException();
    }

    public Task<GetByIdGroupsResponse> GetByIdAsync(string id, bool tracking = false)
    {
        throw new NotImplementedException();
    }

    public Task<DeleteGroupsResponse> DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<UpdateGroupsResponse> UpdateAsync(string id, UpdateGroupRequest request)
    {
        throw new NotImplementedException();
    }
}
using BGU.Core.Entities;

namespace BGU.Infrastructure.Repositories.Interfaces;

public interface IGroupRepository :  IBaseRepository<Group>
{
    public Task<bool> DeleteWithRelationsAsync(string groupId);
}
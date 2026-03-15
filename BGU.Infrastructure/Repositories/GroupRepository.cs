using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BGU.Infrastructure.Repositories;

public class GroupRepository(AppDbContext dbContext) : BaseRepository<Group>(dbContext), IGroupRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<bool> DeleteWithRelationsAsync(string groupId)
    {
        var group = await _dbContext.Groups
            .Include(g => g.Students)
            .ThenInclude(s => s.AppUser)
            .FirstAsync(g => g.Id == groupId);

        _dbContext.AppUsers.RemoveRange(group.Students.Select(s => s.AppUser));

        _dbContext.Groups.Remove(group);

        return await _dbContext.SaveChangesAsync() > 0;
    }
}
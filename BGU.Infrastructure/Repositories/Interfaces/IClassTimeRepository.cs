using BGU.Core.Entities;

namespace BGU.Infrastructure.Repositories.Interfaces;

public interface IClassTimeRepository : IBaseRepository<ClassTime>
{
    Task<bool> BulkCreateAsync(List<ClassTime> classTimes);
}
using BGU.Core.Entities;

namespace BGU.Infrastructure.Repositories.Interfaces;

public interface IClassRepository : IBaseRepository<Class>
{
    Task<bool>BulkCreateAsync(List<Class> classes);
    Task<bool> BulkCreateWithTimesAsync(List<Class> classes, List<ClassTime> classTimes);

}
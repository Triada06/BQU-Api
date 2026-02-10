using BGU.Core.Entities;

namespace BGU.Infrastructure.Repositories.Interfaces;

public interface IColloquiumRepository : IBaseRepository<Colloquiums>
{
    Task<bool> BulkCreateAsync(List<Colloquiums> colloquiums);
}
using BGU.Core.Entities;

namespace BGU.Infrastructure.Repositories.Interfaces;

public interface IIndependentWorkRepository : IBaseRepository<IndependentWork>
{
    Task<bool> BulkCreateAsync(List<IndependentWork> independentWorks);
}
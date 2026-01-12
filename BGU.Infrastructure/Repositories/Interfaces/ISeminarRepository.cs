using BGU.Core.Entities;

namespace BGU.Infrastructure.Repositories.Interfaces;

public interface ISeminarRepository : IBaseRepository<Seminar>
{
    Task<bool> BulkCreate(List<Seminar> seminars);
}
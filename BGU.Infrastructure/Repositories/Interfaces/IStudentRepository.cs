using BGU.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace BGU.Infrastructure.Repositories.Interfaces;

public interface IStudentRepository : IBaseRepository<Student>
{
    Task<Student?> GetByUserId(string userId, Func<IQueryable<Student>,
        IIncludableQueryable<Student, object>>? include = null, bool tracking = true);
}
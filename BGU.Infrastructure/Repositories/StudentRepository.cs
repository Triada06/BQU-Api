using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace BGU.Infrastructure.Repositories;

public class StudentRepository(AppDbContext context) : BaseRepository<Student>(context), IStudentRepository
{
    private readonly AppDbContext _contextToUse = context;

    public async Task<Student?> GetByUserId(string userId,
        Func<IQueryable<Student>, IIncludableQueryable<Student, object>>? include = null, bool tracking = true)
    {
        IQueryable<Student> query = _contextToUse.Students;
        
        query = tracking ? query : query.AsNoTracking();
        
        if (include != null)
            query = include(query);
        
        return await query.FirstOrDefaultAsync(m => m.AppUserId == userId);
    }
}
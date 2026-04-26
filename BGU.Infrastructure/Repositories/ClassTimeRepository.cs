using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Infrastructure.Repositories;

public class ClassTimeRepository(AppDbContext db) : BaseRepository<ClassTime>(db), IClassTimeRepository
{
    private readonly AppDbContext _db = db;

    public async Task<bool> BulkCreateAsync(List<ClassTime> classTimes)
    {
        await _db.ClassTimes.AddRangeAsync(classTimes);
        return await _db.SaveChangesAsync() > 0;
    }
}
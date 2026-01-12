using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Infrastructure.Repositories;

public class SeminarRepository(AppDbContext context) : BaseRepository<Seminar>(context), ISeminarRepository
{
    private readonly AppDbContext _db = context;

    public async Task<bool> BulkCreate(List<Seminar> seminars)
    {
        await _db.Seminars.AddRangeAsync(seminars);
        return await _db.SaveChangesAsync() > 0;
    }
}
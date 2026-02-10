using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Infrastructure.Repositories;

public class ColloquiumRepository(AppDbContext context) : BaseRepository<Colloquiums>(context), IColloquiumRepository
{
    private readonly AppDbContext _context1 = context;

    public async Task<bool> BulkCreateAsync(List<Colloquiums> colloquiums)
    {
        await _context1.Colloquiums.AddRangeAsync(colloquiums);
        return await _context1.SaveChangesAsync() > 0;
    }
}
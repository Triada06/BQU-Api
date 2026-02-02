using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Infrastructure.Repositories;

public class IndependentWorkRepository(AppDbContext context)
    : BaseRepository<IndependentWork>(context), IIndependentWorkRepository
{
    private readonly AppDbContext _context1 = context;

    public async Task<bool> BulkCreateAsync(List<IndependentWork> independentWorks)
    {
        await _context1.IndependentWorks.AddRangeAsync(independentWorks);
        return await _context1.SaveChangesAsync() > 0;
    }
}
using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Infrastructure.Repositories;

public class ClassRepository(AppDbContext context) : BaseRepository<Class>(context), IClassRepository
{
    private readonly AppDbContext _context1 = context;

    public async Task<bool> BulkCreateAsync(List<Class> classes)
    {
        _context1.Classes.AddRange(classes);
        await _context1.SaveChangesAsync();
        return true;
    }
}
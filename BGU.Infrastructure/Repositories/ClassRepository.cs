using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Infrastructure.Repositories;

public class ClassRepository(AppDbContext context) : BaseRepository<Class>(context), IClassRepository
{
    public async Task<bool> BulkCreateAsync(List<Class> classes)
    {
        context.Classes.AddRange(classes);
        await context.SaveChangesAsync();
        return true;
    }
}
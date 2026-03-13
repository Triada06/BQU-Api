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
    public async Task<bool> BulkCreateWithTimesAsync(List<Class> classes, List<ClassTime> classTimes)
    {
        Console.WriteLine($"ClassTimes to insert: {classTimes.Count}");
        Console.WriteLine($"Classes to insert: {classes.Count}");
        foreach (var c in classes)
            Console.WriteLine($"  Class: {c.ClassType} TimeId={c.ClassTimeId} NavNull={c.ClassTime is null}");
    
        foreach (var c in classes)
            c.ClassTime = null;
    
        await _context1.ClassTimes.AddRangeAsync(classTimes);
        await _context1.Classes.AddRangeAsync(classes);
        await _context1.SaveChangesAsync();
        return true;
    }
}
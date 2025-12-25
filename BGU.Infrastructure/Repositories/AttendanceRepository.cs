using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Infrastructure.Repositories;

public class AttendanceRepository(AppDbContext context) : BaseRepository<Attendance>(context),IAttendanceRepository
{
    private readonly AppDbContext _context1 = context;

    public async Task<bool> BulkCreateAsync(List<Attendance> attendances)
    {
        await _context1.AddRangeAsync(attendances);
        return await _context1.SaveChangesAsync() > 0;
    }
}
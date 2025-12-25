using BGU.Core.Entities;

namespace BGU.Infrastructure.Repositories.Interfaces;

public interface IAttendanceRepository : IBaseRepository<Attendance>
{
    Task<bool> BulkCreateAsync(List<Attendance> attendances);
}
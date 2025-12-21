using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Infrastructure.Repositories;

public class ClassTimeRepository(AppDbContext db) : BaseRepository<ClassTime>(db), IClassTimeRepository
{
}
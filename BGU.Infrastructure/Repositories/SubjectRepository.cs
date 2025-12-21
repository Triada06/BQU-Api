using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Infrastructure.Repositories;

public class SubjectRepository(AppDbContext context) : BaseRepository<Subject>(context), ISubjectRepository
{
}
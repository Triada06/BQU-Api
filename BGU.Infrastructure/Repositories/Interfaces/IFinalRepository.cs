using BGU.Core.Entities;

namespace BGU.Infrastructure.Repositories.Interfaces;

public interface IFinalRepository : IBaseRepository<Exam>
{
    Task<bool> ToggleExamEligibilityAsync(string studentId, string subjectId, bool toggle);
}
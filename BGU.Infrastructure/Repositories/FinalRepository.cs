using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BGU.Infrastructure.Repositories;

public class FinalRepository(AppDbContext context) : BaseRepository<Exam>(context), IFinalRepository
{
    private readonly AppDbContext _contextToUse = context;

    public async Task<bool> ToggleExamEligibilityAsync(string studentId, string subjectId, bool toggle)
    {
        var exam = await _contextToUse.Exams.Where(x => x.StudentId == studentId && x.TaughtSubjectId == subjectId)
            .ExecuteUpdateAsync(x => x.SetProperty(e => e.IsAllowed, toggle));
        return exam > 0;
    }
}
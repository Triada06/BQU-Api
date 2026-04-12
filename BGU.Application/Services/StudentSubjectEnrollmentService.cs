using BGU.Application.Dtos.StudentEnrollment;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Application.Services;

public class StudentSubjectEnrollmentService(IStudentSubjectEnrollmentRepository repo, AppDbContext context)
    : IStudentSubjectEnrollmentService
{
    public async Task<StudentSubjectEnrollment> CreateAsync(CreateStudentSubjectEnrollmentDto dto)
    {
        int attempt = dto.Attempt ?? 1;

        if (dto.Attempt == null)
        {
            var existingAttempts = await repo.GetAllAsync();

            attempt = existingAttempts
                .Where(x => x.StudentId == dto.StudentId &&
                            x.TaughtSubject.SubjectId ==
                            context.TaughtSubjects
                                .Where(t => t.Id == dto.TaughtSubjectId)
                                .Select(t => t.SubjectId)
                                .FirstOrDefault())
                .Select(x => x.Attempt)
                .DefaultIfEmpty(0)
                .Max() + 1;
        }

        var entity = new StudentSubjectEnrollment
        {
            StudentId = dto.StudentId,
            TaughtSubjectId = dto.TaughtSubjectId,
            Attempt = attempt
        };

        return await repo.CreateAsync(entity);
    }

    public async Task<List<StudentSubjectEnrollment>> GetAllAsync()
    {
        return await repo.GetAllAsync();
    }

    public async Task<StudentSubjectEnrollment?> GetAsync(string studentId, string subjectId, int attempt)
    {
        return await repo.GetAsync(studentId, subjectId, attempt);
    }

    public async Task<bool> UpdateAsync(
        string studentId,
        string taughtSubjectId,
        int attempt,
        UpdateStudentSubjectEnrollmentDto dto)
    {
        var entity = await repo.GetAsync(studentId, taughtSubjectId, attempt);
        if (entity == null) return false;

        await repo.DeleteAsync(entity);

        var newEntity = new StudentSubjectEnrollment
        {
            StudentId = studentId,
            TaughtSubjectId = dto.TaughtSubjectId,
            Attempt = attempt
        };

        await repo.CreateAsync(newEntity);

        return true;
    }

    public async Task<bool> DeleteAsync(string studentId, string subjectId, int attempt)
    {
        var entity = await repo.GetAsync(studentId, subjectId, attempt);
        if (entity == null) return false;

        await repo.DeleteAsync(entity);
        return true;
    }
}
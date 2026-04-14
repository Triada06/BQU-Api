using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BGU.Infrastructure.Repositories;

public class StudentSubjectEnrollmentRepository(AppDbContext context) : IStudentSubjectEnrollmentRepository
{
    public async Task<StudentSubjectEnrollment> CreateAsync(StudentSubjectEnrollment entity)
    {
        context.StudentSubjectEnrollments.Add(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<List<StudentSubjectEnrollment>> GetAllAsync()
    {
        return await context.StudentSubjectEnrollments
            .Include(x => x.TaughtSubject)
            .ToListAsync();
    }

    public async Task<StudentSubjectEnrollment?> GetAsync(string studentId, string subjectId, int attempt)
    {
        return await context.StudentSubjectEnrollments
            .Include(x => x.TaughtSubject)
            .FirstOrDefaultAsync(x =>
                x.StudentId == studentId &&
                x.TaughtSubjectId == subjectId &&
                x.Attempt == attempt);
    }

    public async Task UpdateAsync(StudentSubjectEnrollment entity)
    {
        context.StudentSubjectEnrollments.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(StudentSubjectEnrollment entity)
    {
        context.StudentSubjectEnrollments.Remove(entity);
        await context.SaveChangesAsync();
    }
}
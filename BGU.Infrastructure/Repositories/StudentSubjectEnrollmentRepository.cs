using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BGU.Infrastructure.Repositories;

public class StudentSubjectEnrollmentRepository : IStudentSubjectEnrollmentRepository
{
    private readonly AppDbContext _context;

    public StudentSubjectEnrollmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StudentSubjectEnrollment> CreateAsync(StudentSubjectEnrollment entity)
    {
        _context.StudentSubjectEnrollments.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<List<StudentSubjectEnrollment>> GetAllAsync()
    {
        return await _context.StudentSubjectEnrollments
            // .Include(x => x.Student)
            .Include(x => x.TaughtSubject)
            // .ThenInclude(x => x.Group)
            .ToListAsync();
    }

    public async Task<StudentSubjectEnrollment?> GetAsync(string studentId, string subjectId, int attempt)
    {
        return await _context.StudentSubjectEnrollments
            // .Include(x => x.Student)
            .Include(x => x.TaughtSubject)
            // .ThenInclude(x => x.Group)
            .FirstOrDefaultAsync(x =>
                x.StudentId == studentId &&
                x.TaughtSubjectId == subjectId &&
                x.Attempt == attempt);
    }

    public async Task UpdateAsync(StudentSubjectEnrollment entity)
    {
        _context.StudentSubjectEnrollments.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(StudentSubjectEnrollment entity)
    {
        _context.StudentSubjectEnrollments.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
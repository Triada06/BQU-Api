using BGU.Core.Entities;

namespace BGU.Infrastructure.Repositories.Interfaces;

public interface IStudentSubjectEnrollmentRepository
{
    Task<StudentSubjectEnrollment> CreateAsync(StudentSubjectEnrollment entity);
    Task<List<StudentSubjectEnrollment>> GetAllAsync();
    Task<StudentSubjectEnrollment?> GetAsync(string studentId, string subjectId, int attempt);
    Task UpdateAsync(StudentSubjectEnrollment entity);
    Task DeleteAsync(StudentSubjectEnrollment entity);
}
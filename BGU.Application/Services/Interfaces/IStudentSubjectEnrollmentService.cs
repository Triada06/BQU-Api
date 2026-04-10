using BGU.Application.Dtos.StudentEnrollment;
using BGU.Core.Entities;

namespace BGU.Application.Services.Interfaces;

public interface IStudentSubjectEnrollmentService
{
    Task<StudentSubjectEnrollment> CreateAsync(CreateStudentSubjectEnrollmentDto dto);
    Task<List<StudentSubjectEnrollment>> GetAllAsync();
    Task<StudentSubjectEnrollment?> GetAsync(string studentId, string subjectId, int attempt);
    Task<bool> UpdateAsync(string studentId, string subjectId, int attempt, UpdateStudentSubjectEnrollmentDto dto);
    Task<bool> DeleteAsync(string studentId, string subjectId, int attempt);
}
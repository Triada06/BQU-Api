using BGU.Application.Dtos.AdmissionYear;
using BGU.Application.Dtos.Class;
using BGU.Application.Dtos.ClassTime;
using BGU.Application.Dtos.Department;
using BGU.Application.Dtos.Faculty;
using BGU.Application.Dtos.Group;
using BGU.Application.Dtos.LectureHall;
using BGU.Application.Dtos.Specialization;
using BGU.Application.Dtos.Student;
using BGU.Application.Dtos.Subject;
using BGU.Application.Dtos.TaughtSubject;
using BGU.Application.Dtos.Teacher;

namespace BGU.Application.Services.Interfaces;

public interface IExcelService
{
    Task<List<AdmissionYearDto>> ParseAdmissionYearExcelAsync(Stream fileStream);
    Task<byte[]> GenerateAdmissionYearTemplateAsync();
    Task<List<FacultyDto>> ParseFacultyExcelAsync(Stream fileStream);
    Task<byte[]> GenerateFacultyTemplateAsync(); // Department
    Task<List<DepartmentDto>> ParseDepartmentExcelAsync(Stream fileStream);
    Task<byte[]> GenerateDepartmentTemplateAsync();

    // Specialization
    Task<List<SpecializationDto>> ParseSpecializationExcelAsync(Stream fileStream);
    Task<byte[]> GenerateSpecializationTemplateAsync();

    // Group
    Task<List<GroupDto>> ParseGroupExcelAsync(Stream fileStream);
    Task<byte[]> GenerateGroupTemplateAsync();

    // Subject
    Task<List<SubjectDto>> ParseSubjectExcelAsync(Stream fileStream);
    Task<byte[]> GenerateSubjectTemplateAsync();

    // LectureHall
    Task<List<LectureHallDto>> ParseLectureHallExcelAsync(Stream fileStream);
    Task<byte[]> GenerateLectureHallTemplateAsync();

    // ClassTime
    Task<List<ClassTimeDto>> ParseClassTimeExcelAsync(Stream fileStream);
    Task<byte[]> GenerateClassTimeTemplateAsync();

    // TaughtSubject
    Task<List<TaughtSubjectDto>> ParseTaughtSubjectExcelAsync(Stream fileStream);
    Task<byte[]> GenerateTaughtSubjectTemplateAsync();

    // Teacher
    Task<List<TeacherDto>> ParseTeacherExcelAsync(Stream fileStream);
    Task<byte[]> GenerateTeacherTemplateAsync();

    Task<List<CreateStudentDto>> ParseStudentExcelAsync(Stream fileStream);
    Task<byte[]> GenerateStudentTemplateAsync();

    Task<List<ClassExcelDto>> ParseClassExcelAsync(Stream fileStream);
    Task<byte[]> GenerateClassTemplateAsync();
}
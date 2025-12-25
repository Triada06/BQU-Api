using BGU.Application.Common;
using BGU.Application.Dtos.Student;

namespace BGU.Application.Services.Interfaces;

public interface IAdminService
{
    Task<ApiResult<StudentCreatedDto>> CreateStudentAsync(CreateStudentDto dto);

    Task<byte[]> BulkImportStudentsAsync(List<CreateStudentDto> students);
    // Task CrateStudent(StudetnCreateDto);
    // Task UpdateStudent(StudetnUpdateDto);
    // Task DeleteStudent(string studentId);
    // Task<IEnumerable<StudentDto>> GetAllStudents();
    // Task<StudentDto> GetStudent(string studentId);
    //
    // Task CrateTeacher(TeacherCreateDto);
    // Task UpdateTeacher(StudetnUpdateDto);
    // Task DeleteTeacher(string teacherId);
    // Task<IEnumerable<TeacherDto>> GetAllTeachers();
    // Task<TeacherDto> GetTeacher(string teacherId);
    //
    // Task CrateDean(DeanCreateDto);
    // Task UpdateTeacher(StudetnUpdateDto);
    // Task DeleteDean(string deanId);
}
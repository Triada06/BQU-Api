using BGU.Application.Common;
using BGU.Application.Dtos.Student;
using BGU.Application.Dtos.Teacher;

namespace BGU.Application.Services.Interfaces;

public interface IAdminService {
    Task<ApiResult<UserCreatedDto>> CreateStudentAsync(StudentDto dto);
    Task<List<BulkImportResult>> BulkImportStudentsAsync(List<StudentDto> students);
    Task<List<BulkImportResult>> BulkImportTeachersAsync(List<TeacherDto> teachers);
}
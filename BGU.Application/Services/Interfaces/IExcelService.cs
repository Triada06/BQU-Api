using BGU.Application.Common;
using BGU.Application.Dtos.Student;
using BGU.Application.Dtos.Teacher;

namespace BGU.Application.Services.Interfaces;

public interface IExcelService
{
    Task<List<TeacherDto>> ParseTeacherExcelAsync(Stream fileStream);

    Task<List<StudentDto>> ParseStudentExcelAsync(Stream fileStream);
    
    byte[] GenerateUserResultsExcel(List<BulkImportResult> results);
}
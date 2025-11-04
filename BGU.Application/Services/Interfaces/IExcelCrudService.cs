using BGU.Application.Common;
using BGU.Application.Dtos.AdmissionYear;
using BGU.Application.Dtos.ClassTime;
using BGU.Application.Dtos.Department;
using BGU.Application.Dtos.Faculty;
using BGU.Application.Dtos.Group;
using BGU.Application.Dtos.LectureHall;
using BGU.Application.Dtos.Specialization;
using BGU.Application.Dtos.Subject;
using BGU.Application.Dtos.TaughtSubject;
using BGU.Application.Dtos.Teacher;

namespace BGU.Application.Services.Interfaces;

public interface IExcelCrudService
{
    Task<List<BulkImportResult>> ProcessAdmissionYearsAsync(List<AdmissionYearDto> items);
    Task<List<BulkImportResult>> ProcessFacultiesAsync(List<FacultyDto> items);
    Task<List<BulkImportResult>> ProcessDepartmentsAsync(List<DepartmentDto> items);
    Task<List<BulkImportResult>> ProcessSpecializationsAsync(List<SpecializationDto> items);
    Task<List<BulkImportResult>> ProcessGroupsAsync(List<GroupDto> items);
    Task<List<BulkImportResult>> ProcessSubjectsAsync(List<SubjectDto> items);
    Task<List<BulkImportResult>> ProcessLectureHallsAsync(List<LectureHallDto> items);
    Task<List<BulkImportResult>> ProcessClassTimesAsync(List<ClassTimeDto> items);
    Task<List<BulkImportResult>> ProcessTaughtSubjectsAsync(List<TaughtSubjectDto> items);
    Task<List<BulkImportResult>> ProcessTeachersAsync(List<TeacherDto> items);
}
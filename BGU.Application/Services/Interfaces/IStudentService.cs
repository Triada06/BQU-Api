using BGU.Application.Common;
using BGU.Application.Contracts.Student.Requests;
using BGU.Application.Contracts.Student.Responses;
using BGU.Application.Dtos.IndependentWorks;
using BGU.Application.Dtos.Student;

namespace BGU.Application.Services.Interfaces;

public interface IStudentService
{
    Task<StudentDashboardResponse> Dashboard(string userId);
    Task<StudentScheduleResponse> GetSchedule(string userId, StudentScheduleRequest request);
    Task<StudentGradesResponse> GetGrades(string userId, StudentGradesRequest request);
    Task<StudentProfileResponse> GetProfile(string userId);
    Task<GetStudentResponse> FilterAsync(string? groupId, int? year);
    Task<GetStudentResponse> SearchAsync(string? searchString);
    Task<GetStudentResponse> GetAllAsync(int page, int pageSize);
    Task<MarkAbsenceStudentResponse> MarkAbsenceAsync(string studentId, string classId);
    Task<GradeStudentColloquiumResponse> GradeStudentColloquiumAsync(GradeStudentColloquiumRequest request);
    Task<GradeStudentSeminarResponse> GradeSeminarAsync(GradeSeminarRequest request);
    Task<ApiResult<GetIndependentWorksDto>> GetIndependentWorksByUserIdAsync(string studentId, string taughtSubjectId);
    Task<ApiResult<GetStudentPageDto>>  GetByIdAsync(string id,CancellationToken cancellationToken);
    Task<ApiResult<GetAcademicHistoryPageDto>>  GetAcademicHistoryAsync(string id,CancellationToken cancellationToken);
    Task<(double score, bool IsEligible)?> GetStudentSubjectScoreAsync(string id, string taughtSubjectId);
    Task<ApiResult<GetStudentFinals>> GetFinalsAsync(string userId);
    Task<ApiResult<bool>> DeleteAsync(string id);
}
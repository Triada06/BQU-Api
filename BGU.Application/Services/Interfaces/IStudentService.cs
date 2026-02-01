using BGU.Application.Contracts.IndependentWorks.Requests;
using BGU.Application.Contracts.Student;
using BGU.Application.Contracts.Student.Requests;
using BGU.Application.Contracts.Student.Responses;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GradeIndependentWorkRequest = BGU.Application.Contracts.Student.Requests.GradeIndependentWorkRequest;

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
    Task<GradeStudentIndependentWorkResponse> GradeIndependentWorkAsync(GradeIndependentWorkRequest request);
    Task<GradeStudentSeminarResponse> GradeSeminarAsync(GradeSeminarRequest request);
}
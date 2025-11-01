using BGU.Application.Contracts.Student;

namespace BGU.Application.Services.Interfaces;

public interface IStudentService
{
    Task<StudentGetTodayClassesResponse> GetTodayClassesAsync(string userId);
        
}
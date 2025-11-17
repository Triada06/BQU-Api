using BGU.Application.Dtos.Student;

namespace BGU.Application.Contracts.Student.Responses;

public record StudentDashboardResponse(
   StudentDashboardDto? Items,
    string Message,
    bool IsSucceeded,
    int StatusCode);
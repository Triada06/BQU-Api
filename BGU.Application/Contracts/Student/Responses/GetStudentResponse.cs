using BGU.Application.Dtos.Student;
using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Student.Responses;

public record GetStudentResponse(
    IEnumerable<GetStudentDto>? Students,
    // int TotalStudentsLeft,
    // int StartOfCurrentStudents,
    // int EndOfCurrentStudents,
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);
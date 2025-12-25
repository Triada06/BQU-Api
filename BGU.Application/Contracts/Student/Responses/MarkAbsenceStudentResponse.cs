using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Student.Responses;

public sealed record MarkAbsenceStudentResponse(StatusCode StatusCode, bool IsSucceeded, string ResponseMessage);
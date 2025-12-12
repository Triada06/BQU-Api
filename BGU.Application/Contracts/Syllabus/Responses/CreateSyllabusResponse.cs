using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Syllabus.Responses;

public sealed record CreateSyllabusResponse(
    string? Id,
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);
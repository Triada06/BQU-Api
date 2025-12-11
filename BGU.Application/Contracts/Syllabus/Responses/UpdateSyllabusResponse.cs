using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Syllabus.Responses;

public sealed record UpdateSyllabusResponse(
    string Id,
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);
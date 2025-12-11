using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Syllabus.Responses;

public sealed record DeleteSyllabusResponse(
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);
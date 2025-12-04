using BGU.Application.Dtos.TaughtSubject;
using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.TaughtSubjects.Responses;

public record GetAllTaughtSubjectResponse(
    IEnumerable<GetTaughtSubjectDto>? Dto,
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);
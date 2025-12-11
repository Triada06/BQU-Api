using BGU.Application.Dtos.Syllabus;
using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Syllabus.Responses;

public sealed record GetByIdSyllabusResponse(
    GetSyllabusDto Dto,
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);
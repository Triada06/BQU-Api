using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.ClassTime.Responses;

public record CreateClassTimeResponse(
    Core.Entities.ClassTime? ClassTime,
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);
using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Rooms.Responses;

public record DeleteRoomResponse(
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);
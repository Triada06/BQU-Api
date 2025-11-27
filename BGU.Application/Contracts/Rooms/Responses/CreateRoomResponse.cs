using BGU.Application.Dtos;
using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Rooms.Responses;

public sealed record CreateRoomResponse(
    RoomDto? Room,
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);
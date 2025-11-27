using BGU.Application.Dtos;
using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Rooms.Responses;

public sealed record UpdateRoomResponse(
    RoomDto? RoomDto,
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);
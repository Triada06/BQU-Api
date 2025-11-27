using BGU.Application.Dtos;
using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Rooms.Responses;

public record GetRoomByIdResponse(
    RoomDto? RoomDtos,
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);
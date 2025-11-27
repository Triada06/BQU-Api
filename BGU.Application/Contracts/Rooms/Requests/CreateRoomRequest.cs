using BGU.Core.Enums;

namespace BGU.Application.Contracts.Rooms.Requests;

public record CreateRoomRequest(string RoomName, int Capacity, RoomType RoomType);
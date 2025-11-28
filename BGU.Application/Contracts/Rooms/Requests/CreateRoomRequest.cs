using BGU.Core.Enums;

namespace BGU.Application.Contracts.Rooms.Requests;

public sealed record CreateRoomRequest(string RoomName, int Capacity, RoomType RoomType);
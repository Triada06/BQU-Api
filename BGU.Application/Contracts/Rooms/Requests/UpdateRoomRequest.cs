using BGU.Core.Enums;

namespace BGU.Application.Contracts.Rooms.Requests;

public sealed record UpdateRoomRequest(string Name, int Capacity);
namespace BGU.Application.Dtos;

public sealed record RoomDto(
    string Id,
    string RoomName,
    int Capacity,
    string RoomType
);
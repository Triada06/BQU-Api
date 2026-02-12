namespace BGU.Application.Dtos.Class;

public record TodaysClassesDto(
    string Id,
    string Name,
    string ClassType,
    string Professor,
    TimeSpan Start,
    TimeSpan End,
    DateTimeOffset Period,
    string Room,
    string Code);
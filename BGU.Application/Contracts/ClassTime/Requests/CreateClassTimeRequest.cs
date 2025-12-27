using BGU.Core.Enums;

namespace BGU.Application.Contracts.ClassTime.Requests;

public record CreateClassTimeRequest(
    TimeSpan Start,
    TimeSpan End,
    DaysOfTheWeek Day,
    string Room,
    string ClassId,
    DateTimeOffset  ClassDate);
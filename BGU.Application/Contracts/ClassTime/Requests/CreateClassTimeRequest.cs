using BGU.Core.Enums;

namespace BGU.Application.Contracts.ClassTime.Requests;

public record CreateClassTimeRequest(
    string ClassId,
    TimeSpan Start,
    TimeSpan End,
    DaysOfTheWeek Day);
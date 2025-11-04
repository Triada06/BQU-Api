using BGU.Core.Enums;

namespace BGU.Application.Dtos.ClassTime;

public record ClassTimeDto(string? Id, TimeSpan Start, TimeSpan End, DaysOfTheWeek DaysOfTheWeek, string Operation);

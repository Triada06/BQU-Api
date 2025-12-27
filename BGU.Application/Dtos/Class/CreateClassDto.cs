using BGU.Core.Enums;

namespace BGU.Application.Dtos.Class;

public record CreateClassDto(
    TimeSpan Start,
    TimeSpan End,
    DaysOfTheWeek Day,
    string Room,
    Frequency Frequency);
    
    
    
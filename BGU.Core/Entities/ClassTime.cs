using BGU.Core.Enums;

namespace BGU.Core.Entities;

public class ClassTime : BaseEntity
{
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
    public DaysOfTheWeek DaysOfTheWeek { get; set; }
}
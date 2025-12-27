using BGU.Core.Enums;

namespace BGU.Core.Entities;

public class ClassTime : BaseEntity
{
    public bool IsUpperWeek { get; set; }
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
    public DaysOfTheWeek DaysOfTheWeek { get; set; }
    public DateTimeOffset ClassDate { get; set; }
}
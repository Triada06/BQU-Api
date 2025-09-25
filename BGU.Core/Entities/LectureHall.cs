namespace BGU.Core.Entities;

public class LectureHall : BaseEntity
{
    public required string Name { get; set; }
    public int Capacity { get; set; }
}
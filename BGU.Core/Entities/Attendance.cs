namespace BGU.Core.Entities;

public class Attendance : BaseEntity
{
    public string StudentId { get; set; }
    public Student Student { get; set; }

    public string ClassId { get; set; }
    public Class Class { get; set; }

    public DateTime Date { get; set; }

    public bool IsAbsent { get; set; }
}
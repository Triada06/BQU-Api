namespace BGU.Core.Entities;

public class Syllabus : BaseEntity
{
    public string Name { get; set; }
    public string FilePath { get; set; }
    public string? TaughtSubjectId { get; set; }
    public TaughtSubject? TaughtSubject { get; set; }
}   
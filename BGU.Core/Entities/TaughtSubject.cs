namespace BGU.Core.Entities;

public class TaughtSubject : BaseEntity
{
    public string SubjectId { get; set; }
    public Subject Subject { get; set; }
    public string TeacherId { get; set; }
    public Teacher Teacher { get; set; }
    public string GroupId { get; set; }
    public Group Group { get; set; }

    public ICollection<Class> Classes { get; set; } = [];
}
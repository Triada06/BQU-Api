using System.ComponentModel.DataAnnotations.Schema;

namespace BGU.Core.Entities;

public class TaughtSubject : BaseEntity
{
    public string Code { get; set; } //todo: add to the excel
    public string SubjectId { get; set; }
    public Subject Subject { get; set; }
    public string TeacherId { get; set; }
    public Teacher Teacher { get; set; }
    public string GroupId { get; set; }
    public Group Group { get; set; }

    public int Hours { get; set; } //todo: add to the excel
    [NotMapped] public int AbsenceLimit => (Hours / 15) * 2;

    public ICollection<Class> Classes { get; set; } = [];
    public ICollection<ClassSession> ClassSessions { get; set; } = [];
    public ICollection<Colloquiums> Colloquiums { get; set; } = [];
    public ICollection<Seminar> Seminars { get; set; } = [];
    public ICollection<IndependentWork> IndependentWorks { get; set; } = [];
}
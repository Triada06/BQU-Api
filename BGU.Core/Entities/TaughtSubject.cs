using System.ComponentModel.DataAnnotations.Schema;

namespace BGU.Core.Entities;

public class TaughtSubject : BaseEntity
{
    private int _Semester;

    public int Semester
    {
        get => _Semester;
        set
        {
            if (value is < 1 or > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(Semester));
            }

            _Semester = value;
        }
    }

    public string Code { get; set; }
    public bool HasSyllabus { get; set; }
    public string SubjectId { get; set; }
    public Subject Subject { get; set; }
    public string TeacherId { get; set; }
    public Teacher Teacher { get; set; }
    public string GroupId { get; set; }
    public Group Group { get; set; }

    public int Hours { get; set; }
    [NotMapped] public int AbsenceLimit => (Hours / 15) * 2;

    public Syllabus? Syllabus { get; set; }
    public ICollection<Class> Classes { get; set; } = [];
    public ICollection<Colloquiums> Colloquiums { get; set; } = [];
    public ICollection<Seminar> Seminars { get; set; } = [];
    public ICollection<IndependentWork> IndependentWorks { get; set; } = [];
}
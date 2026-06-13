namespace BGU.Core.Entities;

public class StudentSubjectResult : BaseEntity
{
    public string StudentId { get; set; }
    public Student Student { get; set; }

    public string TaughtSubjectId { get; set; }
    public TaughtSubject TaughtSubject { get; set; }

    public double GradeBeforeExam { get; set; }
    public double ExamGrade { get; set; }
    public double FinalGrade { get; private set; }

    public bool IsFinalized { get; set; }
    public bool IsPassed { get; private set; }
    public bool IsExamEligible { get; set; }

    public void UpdateFinalStats()
    {
        FinalGrade = ExamGrade + GradeBeforeExam;
        if (ExamGrade >= 17 || FinalGrade >= 51)
        {
            IsPassed = true;
        }
    }
}
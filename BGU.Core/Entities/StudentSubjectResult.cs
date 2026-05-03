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
    //todo: probably needs a IsExamEligible property. That would help to automatically set exam eligibility on creation 

    public void UpdateFinalGrade()
    {
        FinalGrade = ExamGrade + GradeBeforeExam;
    }
}
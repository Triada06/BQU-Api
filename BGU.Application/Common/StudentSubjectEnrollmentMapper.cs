using BGU.Application.Dtos.StudentEnrollment;
using BGU.Core.Entities;

namespace BGU.Application.Common;

public static class StudentSubjectEnrollmentMapper
{
    public static StudentSubjectEnrollmentDto ToDto(StudentSubjectEnrollment x)
    {
        return new StudentSubjectEnrollmentDto
        {
            StudentId = x.StudentId,
            StudentName = x.Student.AppUser.Name + "" + x.Student.AppUser.Surname,

            TaughtSubjectId = x.TaughtSubjectId,
            SubjectName = x.TaughtSubject.Subject.Name,

            GroupCode = x.TaughtSubject.Group.Code,

            Attempt = x.Attempt
        };
    }
}
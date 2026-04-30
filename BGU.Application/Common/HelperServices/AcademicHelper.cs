using BGU.Application.Common.HelperServices.Interfaces;
using BGU.Core.Entities;
using BGU.Core.Enums;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Application.Common.HelperServices;

public class AcademicHelper(
    IColloquiumRepository colloquiumRepository,
    IStudentRepository studentRepository,
    ISeminarRepository seminarRepository,
    IIndependentWorkRepository independentWorkRepository,
    IDepartmentRepository departmentRepository,
    IAttendanceRepository attendanceRepository
) : IAcademicHelper
{
    public async Task<bool> CreateAcademicRequirementsAsync(List<Class> classes, Student student,
        string taughtSubjectId)
    {
        var seminarTypes = classes.FindAll(x => x.ClassType == ClassType.Семинар);

        var attendances = new List<Attendance>();
        var seminars = new List<Seminar>();
        var independentWorks = new List<IndependentWork>();
        var colloquiums = new List<Colloquiums>();

        foreach (var seminarType in seminarTypes)
        {
            var seminar = new Seminar
            {
                StudentId = student.Id,
                TaughtSubjectId = seminarType.TaughtSubjectId,
                GotAt = seminarType.ClassTime.ClassDate.UtcDateTime,
                Grade = Grade.None
            };
            seminars.Add(seminar);
        }

        if (!await seminarRepository.BulkCreate(seminars))
        {
            return false;
        }

        // For EACH class, create attendance 
        foreach (var classItem in classes)
        {
            var att = new Attendance
                { StudentId = student.Id, ClassId = classItem.Id, IsPresent = false };
            attendances.Add(att);
        }

        if (!await attendanceRepository.BulkCreateAsync(attendances))
        {
            return false;
        }

        //create independent works

        for (int i = 0; i < 5; i++)
        {
            var independentWork = new IndependentWork
            {
                Number = i + 1,
                StudentId = student.Id,
                TaughtSubjectId = taughtSubjectId,
                Grade = Grade.None
            };
            independentWorks.Add(independentWork);
        }

        if (!await independentWorkRepository.BulkCreateAsync(independentWorks))
        {
            return false;
        }

        //create colls
        for (int i = 0; i < 3; i++)
        {
            var coll = new Colloquiums
            {
                OrderNumber = i + 1,
                Grade = Grade.None,
                StudentId = student.Id,
                TaughtSubjectId = taughtSubjectId,
            };
            colloquiums.Add(coll);
        }

        return await colloquiumRepository.BulkCreateAsync(colloquiums);
    }
}
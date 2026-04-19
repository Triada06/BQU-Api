using BGU.Core.Entities;
using BGU.Core.Enums;
using BGU.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BGU.Infrastructure.Workers;

public class FinalBackGroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public FinalBackGroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var dateNow = DateTime.UtcNow;
            if (dateNow.Month is 4 or 11) //TODO: Change to 5 la~ter
            {
                var subjects = await db.TaughtSubjects
                    .Include(ts => ts.Group)
                    .ThenInclude(g => g.Students)
                    .Where(ts => ts.Classes.Any(c => c.ClassTime.ClassDate.Year == dateNow.Year))
                    .ToListAsync(stoppingToken);
                var subjectIds = subjects.Select(s => s.Id).ToList();

                var existingExams = await db.Exams
                    .Where(e => subjectIds.Contains(e.TaughtSubjectId))
                    .Select(e => new { e.StudentId, e.TaughtSubjectId })
                    .ToListAsync(stoppingToken);

                var existingSet = existingExams
                    .Select(e => (e.StudentId, e.TaughtSubjectId))
                    .ToHashSet();


                var exams = new List<Exam>();

                foreach (var subject in subjects)
                {
                    var subjectStudents = subject.Group?.Students ?? [];

                    foreach (var student in subjectStudents)
                    {
                        if (!existingSet.Contains((student.Id, subject.Id)))
                        {
                            exams.Add(new Exam
                            {
                                Grade = -1,
                                Date = null,
                                IsConfirmed = false,
                                StudentId = student.Id,
                                TaughtSubjectId = subject.Id
                            });
                        }
                    }
                }

                db.Exams.AddRange(exams);
                await db.SaveChangesAsync(stoppingToken);
            }


            await Task.Delay(60480000, stoppingToken); // works once per 16.8 hrs
        }
    }
}
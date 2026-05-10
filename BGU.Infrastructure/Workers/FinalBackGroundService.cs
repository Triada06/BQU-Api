using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BGU.Infrastructure.Workers;

public class FinalBackGroundService(
    IServiceProvider serviceProvider,
    ILogger<FinalBackGroundService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();

                var db = scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

                var dateNow = DateTime.UtcNow;

                if (dateNow.Month is 4 or 5 or 6 or 10 or 11 or 12)
                {
                    var subjects = await db.TaughtSubjects
                        .Include(ts => ts.Group)
                        .ThenInclude(g => g.Students)
                        .Where(ts => ts.Classes.Any(c => c.ClassTime.ClassDate.Year == dateNow.Year))
                        .ToListAsync(stoppingToken);

                    var subjectIds = subjects
                        .Select(s => s.Id)
                        .ToList();

                    var existingExams = await db.Exams
                        .Where(e => subjectIds.Contains(e.TaughtSubjectId))
                        .Select(e => new
                        {
                            e.StudentId,
                            e.TaughtSubjectId
                        })
                        .ToListAsync(stoppingToken);

                    var existingSet = existingExams
                        .Select(e => (e.StudentId, e.TaughtSubjectId))
                        .ToHashSet();

                    var exams = new List<Exam>();

                    foreach (var subject in subjects)
                    {
                        var students = subject.Group?.Students ?? [];

                        foreach (var student in students)
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

                    if (exams.Count > 0)
                    {
                        db.Exams.AddRange(exams);
                        await db.SaveChangesAsync(stoppingToken);

                        logger.LogInformation(
                            "Created {Count} exams",
                            exams.Count);
                    }
                }

                await Task.Delay(TimeSpan.FromDays(7), stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create finals");

                await Task.Delay(TimeSpan.FromDays(3), stoppingToken);
            }
        }
    }
}
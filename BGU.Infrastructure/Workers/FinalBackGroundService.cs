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
                        .ToListAsync(stoppingToken);

                    var existingSet = existingExams
                        .Select(e => (e.StudentId, e.TaughtSubjectId))
                        .ToHashSet();

                    var examEligibilityRows = await db.StudentSubjectResults
                        .Where(r => subjectIds.Contains(r.TaughtSubjectId))
                        .Select(r => new
                        {
                            r.StudentId,
                            r.TaughtSubjectId,
                            r.IsExamEligible
                        })
                        .ToListAsync(stoppingToken);

                    var examEligibility = examEligibilityRows
                        .GroupBy(r => (r.StudentId, r.TaughtSubjectId))
                        .ToDictionary(g => g.Key, g => g.First().IsExamEligible);

                    var updatedCount = 0;

                    foreach (var exam in existingExams)
                    {
                        if (!examEligibility.TryGetValue((exam.StudentId, exam.TaughtSubjectId), out var isAllowed) ||
                            exam.IsAllowed == isAllowed)
                        {
                            continue;
                        }

                        exam.IsAllowed = isAllowed;
                        updatedCount++;
                    }

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
                                    IsAllowed = examEligibility.TryGetValue((student.Id, subject.Id), out var isAllowed) &&
                                                isAllowed,
                                    StudentId = student.Id,
                                    TaughtSubjectId = subject.Id
                                });
                            }
                        }
                    }

                    if (exams.Count > 0)
                    {
                        db.Exams.AddRange(exams);
                    }

                    if (exams.Count > 0 || updatedCount > 0)
                    {
                        await db.SaveChangesAsync(stoppingToken);

                        logger.LogInformation(
                            "Created {CreatedCount} exams and updated {UpdatedCount} exam eligibility statuses",
                            exams.Count,
                            updatedCount);
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

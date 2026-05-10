using BGU.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BGU.Infrastructure.Workers;

public class NotificationCleanerBackgroundService(
    IServiceProvider serviceProvider,
    ILogger<NotificationCleanerBackgroundService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var dateToCompare = DateTime.UtcNow.AddMonths(-7);

                var notifications = await db.Notifications
                    .Where(x => x.CreatedAt <= dateToCompare)
                    .ToListAsync(stoppingToken);

                if (notifications.Count != 0)
                {
                    db.Notifications.RemoveRange(notifications);
                    await db.SaveChangesAsync(stoppingToken);

                    logger.LogInformation(
                        "Deleted {Count} old notifications",
                        notifications.Count);
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while cleaning notifications");

                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }
        }
    }
}
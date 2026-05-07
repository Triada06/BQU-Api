using BGU.Core.Entities;

namespace BGU.Infrastructure.Repositories.Interfaces;

public interface INotificationRepository : IBaseRepository<Notification>
{
    Task<bool> BulkMarkAllAsReadAsync(IEnumerable<string> notificationIds);
}
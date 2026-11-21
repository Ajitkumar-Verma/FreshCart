using FreshCart.Common.Notifications;

namespace FreshCart.IService.Notifications;

public interface INotificationService
{
    /// <summary>Lists notifications for the authenticated user.</summary>
    Task<IReadOnlyList<UserNotification>> GetMineAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Marks one owned notification as read.</summary>
    Task<UserNotification> MarkReadAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes one owned notification.</summary>
    Task DeleteAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken = default);
}

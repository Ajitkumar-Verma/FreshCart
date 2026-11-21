using FreshCart.Common.Notifications;

namespace FreshCart.IRepository.Notifications;

public interface INotificationRepository
{
    /// <summary>Returns active notifications for one user.</summary>
    Task<IReadOnlyList<UserNotification>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Marks an owned notification as read and returns it.</summary>
    Task<UserNotification?> MarkReadAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an active notification belonging to a user.</summary>
    Task<bool> SoftDeleteAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken = default);
}

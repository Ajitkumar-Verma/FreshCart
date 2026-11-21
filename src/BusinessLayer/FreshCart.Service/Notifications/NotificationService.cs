using FreshCart.Common.Exceptions;
using FreshCart.Common.Notifications;
using FreshCart.IRepository.Notifications;
using FreshCart.IService.Notifications;

namespace FreshCart.Service.Notifications;

public sealed class NotificationService(INotificationRepository notifications) : INotificationService
{
    public async Task<IReadOnlyList<UserNotification>> GetMineAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await notifications.GetByUserAsync(userId, cancellationToken);
    }

    public async Task<UserNotification> MarkReadAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await notifications.MarkReadAsync(notificationId, userId, cancellationToken)
            ?? throw new NotFoundAppException();
    }

    public async Task DeleteAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken = default)
    {
        if (!await notifications.SoftDeleteAsync(notificationId, userId, cancellationToken))
            throw new NotFoundAppException();
    }
}

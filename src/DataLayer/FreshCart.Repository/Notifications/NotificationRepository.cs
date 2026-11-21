using Dapper;
using FreshCart.Common.Notifications;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Notifications;
using System.Data;

namespace FreshCart.Repository.Notifications;

public sealed class NotificationRepository(IUnitOfWork unitOfWork) : INotificationRepository
{
    public async Task<IReadOnlyList<UserNotification>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Notifications.GetByUser,
                    parameters: new { UserId = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<UserNotification>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<UserNotification?> MarkReadAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Notifications.MarkRead,
                    parameters: new { NotificationId = notificationId, UserId = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<UserNotification>(command);
            }, cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Notifications.SoftDelete,
                    parameters: new { NotificationId = notificationId, UserId = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }
}

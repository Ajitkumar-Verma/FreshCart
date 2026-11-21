using Dapper;
using FreshCart.Common.Marketplace;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Marketplace;
using System.Data;

namespace FreshCart.Repository.Marketplace;

public sealed class ShopMembershipRepository(IUnitOfWork unitOfWork) : IShopMembershipRepository
{
    public async Task<IReadOnlyList<ShopMembership>> GetByShopAsync(Guid shopId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Marketplace.ShopMembershipGetByShop,
                    parameters: new { ShopId = shopId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<ShopMembership>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<ShopMembership?> GetByIdAsync(Guid shopId, Guid membershipId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Marketplace.ShopMembershipGetById,
                    parameters: new { ShopId = shopId, MembershipId = membershipId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<ShopMembership>(command);
            }, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid shopId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Marketplace.ShopMembershipExists,
                    parameters: new { ShopId = shopId, UserId = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }

    public async Task<ShopMembership> CreateAsync(ShopMembership membership, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Marketplace.ShopMembershipCreate,
                    parameters: new { membership.Id, membership.ShopId, membership.UserId, membership.RoleName, CreatedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<ShopMembership>(command);
            }, cancellationToken);
    }

    public async Task<ShopMembership?> UpdateAsync(ShopMembership membership, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Marketplace.ShopMembershipUpdate,
                    parameters: new { membership.Id, membership.ShopId, membership.RoleName, UpdatedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<ShopMembership>(command);
            }, cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(Guid shopId, Guid membershipId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Marketplace.ShopMembershipSoftDelete,
                    parameters: new { ShopId = shopId, MembershipId = membershipId, DeletedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }
}

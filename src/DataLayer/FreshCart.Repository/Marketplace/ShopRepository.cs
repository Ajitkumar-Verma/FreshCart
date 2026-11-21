using Dapper;
using FreshCart.Common.Marketplace;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Marketplace;
using System.Data;

namespace FreshCart.Repository.Marketplace;

public sealed class ShopRepository(IUnitOfWork unitOfWork) : IShopRepository
{
    public async Task<IReadOnlyList<Shop>> GetApprovedAsync(CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Marketplace.ShopGetApproved,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<Shop>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<IReadOnlyList<Shop>> GetManagedByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Marketplace.ShopGetManaged,
                    parameters: new { UserId = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<Shop>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<Shop?> GetByIdAsync(Guid shopId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Marketplace.ShopGetById,
                    parameters: new { ShopId = shopId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<Shop>(command);
            }, cancellationToken);
    }

    public async Task<Shop?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Marketplace.ShopGetBySlug,
                    parameters: new { Slug = slug },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<Shop>(command);
            }, cancellationToken);
    }

    public async Task<bool> CanManageAsync(Guid shopId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Marketplace.ShopCanManage,
                    parameters: new { ShopId = shopId, UserId = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }

    public async Task<Shop> CreateAsync(Shop shop, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Marketplace.ShopCreate,
                    parameters: new { shop.Id, shop.Name, shop.Slug, shop.Description, shop.OwnerUserId, CreatedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<Shop>(command);
            }, cancellationToken);
    }

    public async Task<Shop?> UpdateAsync(Shop shop, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Marketplace.ShopUpdate,
                    parameters: new { shop.Id, shop.Name, shop.Slug, shop.Description, UpdatedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<Shop>(command);
            }, cancellationToken);
    }

    public async Task<Shop?> SetStatusAsync(Guid shopId, byte status, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Marketplace.ShopSetStatus,
                    parameters: new { ShopId = shopId, Status = status, UpdatedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<Shop>(command);
            }, cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(Guid shopId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Marketplace.ShopSoftDelete,
                    parameters: new { ShopId = shopId, DeletedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return await connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }
}

using Dapper;
using FreshCart.Common.Marketplace;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Marketplace;
using System.Data;

namespace FreshCart.Repository.Marketplace;

public sealed class ShopBranchRepository(IUnitOfWork unitOfWork) : IShopBranchRepository
{
    public async Task<IReadOnlyList<ShopBranch>> GetPublicAsync(Guid shopId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Marketplace.ShopBranchGetPublic,
                    parameters: new { ShopId = shopId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<ShopBranch>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<ShopBranch?> GetByIdAsync(Guid shopId, Guid branchId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Marketplace.ShopBranchGetById,
                    parameters: new { ShopId = shopId, BranchId = branchId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<ShopBranch>(command);
            }, cancellationToken);
    }

    public async Task<ShopBranch> CreateAsync(ShopBranch branch, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Marketplace.ShopBranchCreate,
                    parameters: new
                    {
                        branch.Id,
                        branch.ShopId,
                        branch.Name,
                        branch.AddressLine,
                        branch.City,
                        branch.PostalCode,
                        branch.Latitude,
                        branch.Longitude,
                        branch.IsActive,
                        CreatedBy = actorUserId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<ShopBranch>(command);
            }, cancellationToken);
    }

    public async Task<ShopBranch?> UpdateAsync(ShopBranch branch, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Marketplace.ShopBranchUpdate,
                    parameters: new
                    {
                        branch.Id,
                        branch.ShopId,
                        branch.Name,
                        branch.AddressLine,
                        branch.City,
                        branch.PostalCode,
                        branch.Latitude,
                        branch.Longitude,
                        branch.IsActive,
                        UpdatedBy = actorUserId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<ShopBranch>(command);
            }, cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(Guid shopId, Guid branchId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Marketplace.ShopBranchSoftDelete,
                    parameters: new { ShopId = shopId, BranchId = branchId, DeletedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }
}

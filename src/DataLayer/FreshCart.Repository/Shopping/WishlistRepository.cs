using Dapper;
using FreshCart.Common.Persistence;
using FreshCart.Common.Shopping;
using FreshCart.IRepository.Shopping;
using System.Data;

namespace FreshCart.Repository.Shopping;

public sealed class WishlistRepository(IUnitOfWork unitOfWork) : IWishlistRepository
{
    public async Task<IReadOnlyList<Wishlist>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.WishlistGetAll,
                    parameters: new { UserId = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<Wishlist>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<Wishlist?> GetByIdAsync(Guid userId, Guid wishlistId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.WishlistGetById,
                    parameters: new { UserId = userId, WishlistId = wishlistId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<Wishlist>(command);
            }, cancellationToken);
    }

    public async Task<Wishlist?> GetByNameAsync(Guid userId, string name, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.WishlistGetByName,
                    parameters: new { UserId = userId, Name = name },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<Wishlist>(command);
            }, cancellationToken);
    }

    public async Task CreateAsync(Guid wishlistId, Guid userId, string name, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.WishlistCreate,
                    parameters: new { WishlistId = wishlistId, UserId = userId, Name = name, CreatedBy = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }, cancellationToken);
    }

    public async Task UpdateAsync(Guid userId, Guid wishlistId, string name, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.WishlistUpdate,
                    parameters: new { UserId = userId, WishlistId = wishlistId, Name = name, UpdatedBy = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }, cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(Guid userId, Guid wishlistId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.WishlistSoftDelete,
                    parameters: new { UserId = userId, WishlistId = wishlistId, DeletedBy = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }

    public async Task<IReadOnlyList<WishlistItem>> GetItemsAsync(Guid wishlistId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.WishlistItemGetAll,
                    parameters: new { WishlistId = wishlistId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<WishlistItem>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<Guid?> GetItemIdAsync(Guid wishlistId, Guid productId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.WishlistItemGetId,
                    parameters: new { WishlistId = wishlistId, ProductId = productId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<Guid?>(command);
            }, cancellationToken);
    }

    public async Task CreateItemAsync(Guid itemId, Guid wishlistId, Guid productId, Guid userId, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.WishlistItemCreate,
                    parameters: new { ItemId = itemId, WishlistId = wishlistId, ProductId = productId, CreatedBy = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }, cancellationToken);
    }

    public async Task RestoreItemAsync(Guid wishlistId, Guid productId, Guid userId, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.WishlistItemRestore,
                    parameters: new { WishlistId = wishlistId, ProductId = productId, UpdatedBy = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }, cancellationToken);
    }

    public async Task<bool> SoftDeleteItemAsync(Guid wishlistId, Guid productId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.WishlistItemSoftDelete,
                    parameters: new { WishlistId = wishlistId, ProductId = productId, DeletedBy = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }
}

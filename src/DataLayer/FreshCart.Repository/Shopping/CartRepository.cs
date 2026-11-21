using Dapper;
using FreshCart.Common.Persistence;
using FreshCart.Common.Shopping;
using FreshCart.IRepository.Shopping;
using System.Data;

namespace FreshCart.Repository.Shopping;

public sealed class CartRepository(IUnitOfWork unitOfWork) : ICartRepository
{
    public async Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.CartGetByUser,
                    parameters: new { UserId = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<Cart>(command);
            }, cancellationToken);
    }

    public async Task CreateAsync(Guid cartId, Guid userId, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.CartCreate,
                    parameters: new { CartId = cartId, UserId = userId, CreatedBy = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }, cancellationToken);
    }

    public async Task<IReadOnlyList<CartItem>> GetItemsAsync(Guid cartId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.CartItemGetAll,
                    parameters: new { CartId = cartId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<CartItem>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<CartItem?> GetItemByVariantAsync(Guid cartId, Guid productVariantId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.CartItemGetByVariant,
                    parameters: new { CartId = cartId, ProductVariantId = productVariantId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<CartItem>(command);
            }, cancellationToken);
    }

    public async Task CreateItemAsync(Guid itemId, Guid cartId, Guid productVariantId, decimal quantity, Guid userId, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.CartItemCreate,
                    parameters: new
                    {
                        ItemId = itemId,
                        CartId = cartId,
                        ProductVariantId = productVariantId,
                        Quantity = quantity,
                        CreatedBy = userId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }, cancellationToken);
    }

    public async Task UpdateItemAsync(Guid itemId, decimal quantity, Guid userId, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.CartItemUpdate,
                    parameters: new { ItemId = itemId, Quantity = quantity, UpdatedBy = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }, cancellationToken);
    }

    public async Task<bool> SoftDeleteItemAsync(Guid cartId, Guid productVariantId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Shopping.CartItemSoftDelete,
                    parameters: new { CartId = cartId, ProductVariantId = productVariantId, DeletedBy = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }
}

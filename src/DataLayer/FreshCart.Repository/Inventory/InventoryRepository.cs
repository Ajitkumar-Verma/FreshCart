using Dapper;
using FreshCart.Common.Inventory;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Inventory;
using System.Data;

namespace FreshCart.Repository.Inventory;

public sealed class InventoryRepository(IUnitOfWork unitOfWork) : IInventoryRepository
{
    public async Task<IReadOnlyList<InventoryItem>> GetByShopAsync(Guid shopId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Inventory.GetByShop,
                    parameters: new { ShopId = shopId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<InventoryItem>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<InventoryItem?> GetByIdAsync(Guid inventoryId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Inventory.GetById,
                    parameters: new { InventoryId = inventoryId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<InventoryItem>(command);
            }, cancellationToken);
    }

    public async Task<InventoryItem?> GetByBranchAndVariantAsync(Guid branchId, Guid productVariantId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Inventory.GetByBranchAndVariant,
                    parameters: new { ShopBranchId = branchId, ProductVariantId = productVariantId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<InventoryItem>(command);
            }, cancellationToken);
    }

    public async Task<InventoryItem> CreateAsync(Guid id, Guid branchId, Guid productVariantId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Inventory.Create,
                    parameters: new { Id = id, ShopBranchId = branchId, ProductVariantId = productVariantId, CreatedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<InventoryItem>(command);
            }, cancellationToken);
    }

    public async Task<bool> UpdateQuantityAsync(Guid inventoryId, decimal quantityChange, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Inventory.UpdateQuantity,
                    parameters: new { InventoryId = inventoryId, QuantityChange = quantityChange, ChangedByUserId = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }

    public async Task CreateMovementAsync(Guid movementId, Guid inventoryId, decimal quantityChange, string reason, string? notes, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Inventory.CreateStockMovement,
                    parameters: new { Id = movementId, InventoryId = inventoryId, QuantityChange = quantityChange, Reason = reason, Notes = notes, ChangedByUserId = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }, cancellationToken);
    }
}

using Dapper;
using FreshCart.Common.Catalog;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Catalog;
using System.Data;

namespace FreshCart.Repository.Catalog;

/// <summary>SQL Server implementation using Dapper and stored procedures only.</summary>
public sealed class ProductRepository(IUnitOfWork unitOfWork) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetAvailableAsync(Guid? shopId = null, Guid? categoryId = null, string? search = null, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.ProductGetAvailable,
                    parameters: new
                    {
                        ShopId = shopId,
                        CategoryId = categoryId,
                        Search = string.IsNullOrWhiteSpace(search) ? null : search.Trim()
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<Product>(command)).AsList();
            },
            cancellationToken: cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.ProductGetById,
                    parameters: new
                    {
                        ProductId = productId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<Product>(command);
            },
            cancellationToken: cancellationToken);
    }

    public async Task<Product?> GetForManagementAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.ProductGetForManagement,
                    parameters: new { ProductId = productId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<Product>(command);
            }, cancellationToken);
    }

    public async Task<Guid?> GetShopIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.ProductGetShopId,
                    parameters: new { ProductId = productId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<Guid?>(command);
            }, cancellationToken);
    }

    public async Task CreateAsync(Guid productId, Guid shopId, Guid categoryId, string name, string? description, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.ProductCreate,
                    parameters: new
                    {
                        ProductId = productId,
                        ShopId = shopId,
                        CategoryId = categoryId,
                        Name = name,
                        Description = description,
                        CreatedBy = actorUserId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }, cancellationToken);
    }

    public async Task CreateVariantAsync(ProductVariant variant, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.ProductVariantCreate,
                    parameters: new
                    {
                        variant.Id,
                        variant.ProductId,
                        variant.SKU,
                        variant.UnitName,
                        variant.UnitQuantity,
                        variant.UnitPrice,
                        variant.CurrencyCode,
                        variant.IsDefault,
                        CreatedBy = actorUserId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }, cancellationToken);
    }

    public async Task CreatePriceHistoryAsync(Guid priceHistoryId, Guid productVariantId, decimal? oldPrice, decimal newPrice, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.ProductPriceHistoryCreate,
                    parameters: new
                    {
                        Id = priceHistoryId,
                        ProductVariantId = productVariantId,
                        OldPrice = oldPrice,
                        NewPrice = newPrice,
                        ChangedByUserId = actorUserId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }, cancellationToken);
    }

    public async Task UpdateAsync(Guid productId, Guid categoryId, string name, string? description, bool isActive, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.ProductUpdate,
                    parameters: new
                    {
                        ProductId = productId,
                        CategoryId = categoryId,
                        Name = name,
                        Description = description,
                        IsActive = isActive,
                        UpdatedBy = actorUserId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }, cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(Guid productId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.ProductSoftDelete,
                    parameters: new { ProductId = productId, DeletedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return await connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductVariant>> GetVariantsAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.ProductVariantGetAll,
                    parameters: new { ProductId = productId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<ProductVariant>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductVariant>> GetVariantsForManagementAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.ProductVariantGetForManagement,
                    parameters: new { ProductId = productId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<ProductVariant>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<ProductVariant?> GetVariantByIdAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.ProductVariantGetById,
                    parameters: new { VariantId = variantId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<ProductVariant>(command);
            }, cancellationToken);
    }

    public async Task<Guid?> GetProductIdByVariantIdAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.ProductVariantGetProductId,
                    parameters: new { VariantId = variantId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<Guid?>(command);
            }, cancellationToken);
    }

    public async Task ClearDefaultVariantAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.ProductVariantClearDefault,
                    parameters: new { ProductId = productId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }, cancellationToken);
    }

    public async Task UpdateVariantAsync(ProductVariant variant, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.ProductVariantUpdate,
                    parameters: new
                    {
                        variant.Id,
                        variant.ProductId,
                        variant.SKU,
                        variant.UnitName,
                        variant.UnitQuantity,
                        variant.UnitPrice,
                        variant.CurrencyCode,
                        variant.IsDefault,
                        variant.IsActive,
                        UpdatedBy = actorUserId
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                await connection.ExecuteAsync(command);
            }, cancellationToken);
    }

    public async Task<bool> SoftDeleteVariantAsync(Guid variantId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.ProductVariantSoftDelete,
                    parameters: new { VariantId = variantId, DeletedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return await connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }
}


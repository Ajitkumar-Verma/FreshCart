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
        var connection = await unitOfWork.GetConnectionAsync(cancellationToken);
        var command = new CommandDefinition(StoredProcedure.Catalog.ProductGetAvailable,
            new { ShopId = shopId, CategoryId = categoryId, Search = string.IsNullOrWhiteSpace(search) ? null : search.Trim() },
            transaction: unitOfWork.Transaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
        return (await connection.QueryAsync<Product>(command)).AsList();
    }

    public async Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var connection = await unitOfWork.GetConnectionAsync(cancellationToken);
        var command = new CommandDefinition(StoredProcedure.Catalog.ProductGetById,
            new { ProductId = productId }, transaction: unitOfWork.Transaction,
            commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<Product>(command);
    }
}


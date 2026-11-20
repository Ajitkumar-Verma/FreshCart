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
}


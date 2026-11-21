using Dapper;
using FreshCart.Common.Catalog;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Catalog;
using System.Data;

namespace FreshCart.Repository.Catalog;

public sealed class ProductImageRepository(IUnitOfWork unitOfWork) : IProductImageRepository
{
    public async Task<IReadOnlyList<ProductImage>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.ProductImageGetByProduct,
                    parameters: new { ProductId = productId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<ProductImage>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<ProductImage?> GetByIdAsync(Guid productId, Guid imageId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.ProductImageGetById,
                    parameters: new { ProductId = productId, ImageId = imageId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<ProductImage>(command);
            }, cancellationToken);
    }

    public async Task<ProductImage> CreateAsync(ProductImage image, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.ProductImageCreate,
                    parameters: new { image.Id, image.ProductId, image.ImageUrl, image.AltText, image.SortOrder, CreatedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<ProductImage>(command);
            }, cancellationToken);
    }

    public async Task<ProductImage?> UpdateAsync(ProductImage image, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.ProductImageUpdate,
                    parameters: new { image.Id, image.ProductId, image.ImageUrl, image.AltText, image.SortOrder, UpdatedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<ProductImage>(command);
            }, cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(Guid productId, Guid imageId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.ProductImageSoftDelete,
                    parameters: new { ProductId = productId, ImageId = imageId, DeletedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }
}

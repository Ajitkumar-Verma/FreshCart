using Dapper;
using FreshCart.Common.Persistence;
using FreshCart.Common.Reviews;
using FreshCart.IRepository.Reviews;
using System.Data;

namespace FreshCart.Repository.Reviews;

public sealed class ReviewRepository(IUnitOfWork unitOfWork) : IReviewRepository
{
    public async Task<IReadOnlyList<ProductReview>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Reviews.GetByProduct,
                    parameters: new { ProductId = productId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<ProductReview>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<ProductReview?> GetByOrderItemAsync(Guid userId, Guid orderItemId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Reviews.GetByOrderItem,
                    parameters: new { UserId = userId, OrderItemId = orderItemId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<ProductReview>(command);
            }, cancellationToken);
    }

    public async Task<bool> CanReviewOrderItemAsync(Guid userId, Guid productId, Guid orderItemId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Reviews.CanReviewOrderItem,
                    parameters: new { UserId = userId, ProductId = productId, OrderItemId = orderItemId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }

    public async Task<ProductReview> CreateAsync(ProductReview review, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Reviews.Create,
                    parameters: new { review.Id, review.ProductId, review.UserId, review.OrderItemId, review.Rating, review.Comment, review.IsPublished, CreatedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<ProductReview>(command);
            }, cancellationToken);
    }

    public async Task<ProductReview?> UpdateAsync(ProductReview review, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Reviews.Update,
                    parameters: new { review.Id, review.UserId, review.Rating, review.Comment, UpdatedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<ProductReview>(command);
            }, cancellationToken);
    }

    public async Task<ProductReview?> SetPublishedAsync(Guid reviewId, bool isPublished, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Reviews.SetPublished,
                    parameters: new { ReviewId = reviewId, IsPublished = isPublished, UpdatedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<ProductReview>(command);
            }, cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(Guid reviewId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Reviews.SoftDelete,
                    parameters: new { ReviewId = reviewId, UserId = userId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }
}

using FreshCart.Common.Exceptions;
using FreshCart.Common.Persistence;
using FreshCart.Common.Reviews;
using FreshCart.IRepository.Reviews;
using FreshCart.IService.Reviews;

namespace FreshCart.Service.Reviews;

public sealed class ReviewService(IReviewRepository reviews, IIdGenerator idGenerator) : IReviewService
{
    public async Task<IReadOnlyList<ProductReview>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await reviews.GetByProductAsync(productId, cancellationToken);
    }

    public async Task<ProductReview> SubmitAsync(Guid productId, Guid orderItemId, ProductReviewRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        if (request.Rating is < 1 or > 5)
            throw new ValidationAppException();
        if (!await reviews.CanReviewOrderItemAsync(actorUserId, productId, orderItemId, cancellationToken))
            throw new ForbiddenAppException();

        var existing = await reviews.GetByOrderItemAsync(actorUserId, orderItemId, cancellationToken);
        if (existing is not null)
        {
            existing.Rating = request.Rating;
            existing.Comment = request.Comment?.Trim();
            return await reviews.UpdateAsync(existing, actorUserId, cancellationToken) ?? throw new NotFoundAppException();
        }

        return await reviews.CreateAsync(new ProductReview
        {
            Id = idGenerator.NewId(),
            ProductId = productId,
            UserId = actorUserId,
            OrderItemId = orderItemId,
            Rating = request.Rating,
            Comment = request.Comment?.Trim(),
            IsPublished = true
        }, actorUserId, cancellationToken);
    }

    public async Task<ProductReview> SetPublishedAsync(Guid reviewId, bool isPublished, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await reviews.SetPublishedAsync(reviewId, isPublished, actorUserId, cancellationToken)
            ?? throw new NotFoundAppException();
    }

    public async Task DeleteMineAsync(Guid reviewId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        if (!await reviews.SoftDeleteAsync(reviewId, actorUserId, cancellationToken))
            throw new NotFoundAppException();
    }
}

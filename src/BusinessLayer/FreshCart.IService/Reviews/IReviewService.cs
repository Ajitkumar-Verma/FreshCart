using FreshCart.Common.Reviews;

namespace FreshCart.IService.Reviews;

public interface IReviewService
{
    /// <summary>Lists published reviews for a product.</summary>
    Task<IReadOnlyList<ProductReview>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>Creates or updates the authenticated customer's review for a purchased order item.</summary>
    Task<ProductReview> SubmitAsync(Guid productId, Guid orderItemId, ProductReviewRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Changes review visibility as a platform administrator.</summary>
    Task<ProductReview> SetPublishedAsync(Guid reviewId, bool isPublished, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an owned review.</summary>
    Task DeleteMineAsync(Guid reviewId, Guid actorUserId, CancellationToken cancellationToken = default);
}

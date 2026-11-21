using FreshCart.Common.Reviews;

namespace FreshCart.IRepository.Reviews;

public interface IReviewRepository
{
    /// <summary>Returns published, non-deleted reviews for a product.</summary>
    Task<IReadOnlyList<ProductReview>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>Finds the review for a user and purchased order item.</summary>
    Task<ProductReview?> GetByOrderItemAsync(Guid userId, Guid orderItemId, CancellationToken cancellationToken = default);

    /// <summary>Validates ownership and product relationship for a completed purchased order item.</summary>
    Task<bool> CanReviewOrderItemAsync(Guid userId, Guid productId, Guid orderItemId, CancellationToken cancellationToken = default);

    /// <summary>Creates one product review.</summary>
    Task<ProductReview> CreateAsync(ProductReview review, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Updates rating and comment on an existing review.</summary>
    Task<ProductReview?> UpdateAsync(ProductReview review, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Changes published state for an active review.</summary>
    Task<ProductReview?> SetPublishedAsync(Guid reviewId, bool isPublished, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an active review owned by one user.</summary>
    Task<bool> SoftDeleteAsync(Guid reviewId, Guid userId, CancellationToken cancellationToken = default);
}

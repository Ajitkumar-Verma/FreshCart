using FreshCart.Common.Catalog;

namespace FreshCart.IRepository.Catalog;

public interface IProductImageRepository
{
    /// <summary>Lists non-deleted image metadata for a product.</summary>
    Task<IReadOnlyList<ProductImage>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>Finds one image metadata row by product and image identifiers.</summary>
    Task<ProductImage?> GetByIdAsync(Guid productId, Guid imageId, CancellationToken cancellationToken = default);

    /// <summary>Inserts one product image metadata row.</summary>
    Task<ProductImage> CreateAsync(ProductImage image, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Updates one product image metadata row.</summary>
    Task<ProductImage?> UpdateAsync(ProductImage image, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes one product image metadata row.</summary>
    Task<bool> SoftDeleteAsync(Guid productId, Guid imageId, Guid actorUserId, CancellationToken cancellationToken = default);
}

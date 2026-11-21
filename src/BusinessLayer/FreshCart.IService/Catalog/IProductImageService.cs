using FreshCart.Common.Catalog;

namespace FreshCart.IService.Catalog;

public interface IProductImageService
{
    /// <summary>Lists active images for a public product.</summary>
    Task<IReadOnlyList<ProductImage>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>Adds image metadata to a product managed by the caller.</summary>
    Task<ProductImage> CreateAsync(Guid productId, ProductImageRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Updates image metadata for a product managed by the caller.</summary>
    Task<ProductImage> UpdateAsync(Guid productId, Guid imageId, ProductImageRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes one product image metadata row.</summary>
    Task DeleteAsync(Guid productId, Guid imageId, Guid actorUserId, CancellationToken cancellationToken = default);
}

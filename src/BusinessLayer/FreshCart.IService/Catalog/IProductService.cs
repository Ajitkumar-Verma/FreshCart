using FreshCart.Common.Catalog;

namespace FreshCart.IService.Catalog;

public interface IProductService
{
    /// <summary>Returns active products matching optional shop, category, and text filters.</summary>
    Task<IReadOnlyList<Product>> GetAvailableAsync(Guid? shopId = null, Guid? categoryId = null, string? search = null, CancellationToken cancellationToken = default);

    /// <summary>Returns one active product by its public GUID, or null when it does not exist.</summary>
    Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>Returns one product from its manager-facing view after verifying shop ownership.</summary>
    Task<Product> GetManagedByIdAsync(Guid productId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Creates a product with its initial sellable variant after verifying shop and category access.</summary>
    Task<Product> CreateAsync(ProductCreateRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Updates product details after verifying the caller manages its shop.</summary>
    Task<Product> UpdateAsync(Guid productId, ProductUpdateRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes a product and preserves historical order references.</summary>
    Task DeleteAsync(Guid productId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Returns active sale variants for an approved storefront product.</summary>
    Task<IReadOnlyList<ProductVariant>> GetVariantsAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>Returns a product variant by identifier for its manager.</summary>
    Task<ProductVariant> GetVariantForManagementAsync(Guid variantId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Returns a variant only when its product is currently visible in the public catalog.</summary>
    Task<ProductVariant> GetVariantForStoreAsync(Guid variantId, CancellationToken cancellationToken = default);

    /// <summary>Adds a sellable variant and records its initial price.</summary>
    Task<ProductVariant> CreateVariantAsync(Guid productId, ProductVariantRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Updates a variant and records a price-history row when its price changes.</summary>
    Task<ProductVariant> UpdateVariantAsync(Guid variantId, ProductVariantRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes a non-default variant while preserving order history.</summary>
    Task DeleteVariantAsync(Guid variantId, Guid actorUserId, CancellationToken cancellationToken = default);
}

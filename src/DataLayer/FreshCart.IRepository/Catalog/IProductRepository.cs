using FreshCart.Common.Catalog;

namespace FreshCart.IRepository.Catalog;

public interface IProductRepository
{
    /// <summary>Loads active catalog rows by calling the catalog listing stored procedure.</summary>
    Task<IReadOnlyList<Product>> GetAvailableAsync(Guid? shopId = null, Guid? categoryId = null, string? search = null, CancellationToken cancellationToken = default);

    /// <summary>Loads one active catalog row by GUID using the catalog detail stored procedure.</summary>
    Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>Returns a product and default variant without storefront approval filters for owner management.</summary>
    Task<Product?> GetForManagementAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>Returns the active shop that owns a product.</summary>
    Task<Guid?> GetShopIdAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>Inserts the product row only.</summary>
    Task CreateAsync(Guid productId, Guid shopId, Guid categoryId, string name, string? description, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Inserts the initial product variant only.</summary>
    Task CreateVariantAsync(ProductVariant variant, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Inserts one initial price history row for a newly created variant.</summary>
    Task CreatePriceHistoryAsync(Guid priceHistoryId, Guid productVariantId, decimal? oldPrice, decimal newPrice, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Updates the editable product fields only.</summary>
    Task UpdateAsync(Guid productId, Guid categoryId, string name, string? description, bool isActive, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes one product row.</summary>
    Task<bool> SoftDeleteAsync(Guid productId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Returns active variants for a publicly available product.</summary>
    Task<IReadOnlyList<ProductVariant>> GetVariantsAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>Returns non-deleted variants for management, including inactive products.</summary>
    Task<IReadOnlyList<ProductVariant>> GetVariantsForManagementAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>Finds one non-deleted variant by identifier.</summary>
    Task<ProductVariant?> GetVariantByIdAsync(Guid variantId, CancellationToken cancellationToken = default);

    /// <summary>Returns the parent product identifier for a variant.</summary>
    Task<Guid?> GetProductIdByVariantIdAsync(Guid variantId, CancellationToken cancellationToken = default);

    /// <summary>Clears the default flag from active variants of one product.</summary>
    Task ClearDefaultVariantAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>Updates the editable fields of one variant.</summary>
    Task UpdateVariantAsync(ProductVariant variant, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes one non-default variant.</summary>
    Task<bool> SoftDeleteVariantAsync(Guid variantId, Guid actorUserId, CancellationToken cancellationToken = default);
}

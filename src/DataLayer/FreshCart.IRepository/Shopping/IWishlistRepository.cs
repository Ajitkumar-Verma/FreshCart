using FreshCart.Common.Shopping;

namespace FreshCart.IRepository.Shopping;

public interface IWishlistRepository
{
    /// <summary>Returns active wishlists owned by a user.</summary>
    Task<IReadOnlyList<Wishlist>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Finds one active wishlist owned by a user.</summary>
    Task<Wishlist?> GetByIdAsync(Guid userId, Guid wishlistId, CancellationToken cancellationToken = default);

    /// <summary>Finds a wishlist name, including soft-deleted rows, to enforce the unique owner/name key.</summary>
    Task<Wishlist?> GetByNameAsync(Guid userId, string name, CancellationToken cancellationToken = default);

    /// <summary>Inserts one wishlist.</summary>
    Task CreateAsync(Guid wishlistId, Guid userId, string name, CancellationToken cancellationToken = default);

    /// <summary>Updates the name of one active wishlist.</summary>
    Task UpdateAsync(Guid userId, Guid wishlistId, string name, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes one active wishlist owned by a user.</summary>
    Task<bool> SoftDeleteAsync(Guid userId, Guid wishlistId, CancellationToken cancellationToken = default);

    /// <summary>Returns public product details for active entries in a wishlist.</summary>
    Task<IReadOnlyList<WishlistItem>> GetItemsAsync(Guid wishlistId, CancellationToken cancellationToken = default);

    /// <summary>Finds a wishlist entry for a product, including soft-deleted entries.</summary>
    Task<Guid?> GetItemIdAsync(Guid wishlistId, Guid productId, CancellationToken cancellationToken = default);

    /// <summary>Inserts one product entry into a wishlist.</summary>
    Task CreateItemAsync(Guid itemId, Guid wishlistId, Guid productId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Restores one previously removed wishlist entry.</summary>
    Task RestoreItemAsync(Guid wishlistId, Guid productId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes one active wishlist entry.</summary>
    Task<bool> SoftDeleteItemAsync(Guid wishlistId, Guid productId, Guid userId, CancellationToken cancellationToken = default);
}

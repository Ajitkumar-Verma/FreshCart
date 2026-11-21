using FreshCart.Common.Shopping;

namespace FreshCart.IService.Shopping;

public interface IWishlistService
{
    /// <summary>Returns active wishlists belonging to a user.</summary>
    Task<IReadOnlyList<Wishlist>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Creates a named wishlist for a user.</summary>
    Task<Wishlist> CreateAsync(Guid userId, WishlistRequest request, CancellationToken cancellationToken = default);

    /// <summary>Returns one wishlist owned by the user or raises not found.</summary>
    Task<Wishlist> GetByIdAsync(Guid userId, Guid wishlistId, CancellationToken cancellationToken = default);

    /// <summary>Renames one wishlist owned by the user.</summary>
    Task<Wishlist> UpdateAsync(Guid userId, Guid wishlistId, WishlistRequest request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes one wishlist owned by the user.</summary>
    Task DeleteAsync(Guid userId, Guid wishlistId, CancellationToken cancellationToken = default);

    /// <summary>Returns public product details saved in a user's wishlist.</summary>
    Task<IReadOnlyList<WishlistItem>> GetItemsAsync(Guid userId, Guid wishlistId, CancellationToken cancellationToken = default);

    /// <summary>Adds a visible product to a wishlist or restores its previous soft-deleted entry.</summary>
    Task AddItemAsync(Guid userId, Guid wishlistId, Guid productId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes one product entry from a user's wishlist.</summary>
    Task RemoveItemAsync(Guid userId, Guid wishlistId, Guid productId, CancellationToken cancellationToken = default);
}

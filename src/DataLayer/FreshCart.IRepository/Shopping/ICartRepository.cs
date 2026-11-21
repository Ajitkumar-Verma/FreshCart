using FreshCart.Common.Shopping;

namespace FreshCart.IRepository.Shopping;

public interface ICartRepository
{
    /// <summary>Finds the active cart belonging to a user.</summary>
    Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Inserts an empty cart row for a user.</summary>
    Task CreateAsync(Guid cartId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Returns the current active cart items with current prices and availability.</summary>
    Task<IReadOnlyList<CartItem>> GetItemsAsync(Guid cartId, CancellationToken cancellationToken = default);

    /// <summary>Finds a cart item by cart and variant, including soft-deleted rows.</summary>
    Task<CartItem?> GetItemByVariantAsync(Guid cartId, Guid productVariantId, CancellationToken cancellationToken = default);

    /// <summary>Inserts one cart item row.</summary>
    Task CreateItemAsync(Guid itemId, Guid cartId, Guid productVariantId, decimal quantity, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Updates or restores one cart item row.</summary>
    Task UpdateItemAsync(Guid itemId, decimal quantity, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes one cart item belonging to the user's cart.</summary>
    Task<bool> SoftDeleteItemAsync(Guid cartId, Guid productVariantId, Guid userId, CancellationToken cancellationToken = default);
}

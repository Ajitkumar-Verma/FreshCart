using FreshCart.Common.Shopping;

namespace FreshCart.IService.Shopping;

public interface ICartService
{
    /// <summary>Returns the authenticated user's cart, creating an empty cart if needed.</summary>
    Task<Cart> GetAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Adds the requested quantity of a public active product variant to the cart.</summary>
    Task<Cart> AddItemAsync(Guid userId, Guid productVariantId, CartItemRequest request, CancellationToken cancellationToken = default);

    /// <summary>Sets the quantity of one item in the authenticated user's cart.</summary>
    Task<Cart> UpdateItemAsync(Guid userId, Guid productVariantId, CartItemRequest request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes one item from the authenticated user's cart.</summary>
    Task<Cart> RemoveItemAsync(Guid userId, Guid productVariantId, CancellationToken cancellationToken = default);
}

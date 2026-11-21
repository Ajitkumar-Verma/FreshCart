using FreshCart.Common.Exceptions;
using FreshCart.Common.Persistence;
using FreshCart.Common.Shopping;
using FreshCart.IRepository.Shopping;
using FreshCart.IService.Catalog;
using FreshCart.IService.Shopping;

namespace FreshCart.Service.Shopping;

public sealed class WishlistService(
    IWishlistRepository wishlists,
    IProductService products,
    IIdGenerator idGenerator) : IWishlistService
{
    public async Task<IReadOnlyList<Wishlist>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await wishlists.GetAllAsync(userId, cancellationToken);
    }

    public async Task<Wishlist> CreateAsync(Guid userId, WishlistRequest request, CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();
        if (await wishlists.GetByNameAsync(userId, name, cancellationToken) is not null)
            throw new ConflictAppException();
        var wishlistId = idGenerator.NewId();
        await wishlists.CreateAsync(wishlistId, userId, name, cancellationToken);
        return await GetByIdAsync(userId, wishlistId, cancellationToken);
    }

    public async Task<Wishlist> GetByIdAsync(Guid userId, Guid wishlistId, CancellationToken cancellationToken = default)
    {
        return await wishlists.GetByIdAsync(userId, wishlistId, cancellationToken)
            ?? throw new NotFoundAppException();
    }

    public async Task<Wishlist> UpdateAsync(Guid userId, Guid wishlistId, WishlistRequest request, CancellationToken cancellationToken = default)
    {
        _ = await GetByIdAsync(userId, wishlistId, cancellationToken);
        var name = request.Name.Trim();
        var existing = await wishlists.GetByNameAsync(userId, name, cancellationToken);
        if (existing is not null && existing.Id != wishlistId)
            throw new ConflictAppException();
        await wishlists.UpdateAsync(userId, wishlistId, name, cancellationToken);
        return await GetByIdAsync(userId, wishlistId, cancellationToken);
    }

    public async Task DeleteAsync(Guid userId, Guid wishlistId, CancellationToken cancellationToken = default)
    {
        _ = await GetByIdAsync(userId, wishlistId, cancellationToken);
        if (!await wishlists.SoftDeleteAsync(userId, wishlistId, cancellationToken))
            throw new NotFoundAppException();
    }

    public async Task<IReadOnlyList<WishlistItem>> GetItemsAsync(Guid userId, Guid wishlistId, CancellationToken cancellationToken = default)
    {
        _ = await GetByIdAsync(userId, wishlistId, cancellationToken);
        return await wishlists.GetItemsAsync(wishlistId, cancellationToken);
    }

    public async Task AddItemAsync(Guid userId, Guid wishlistId, Guid productId, CancellationToken cancellationToken = default)
    {
        _ = await GetByIdAsync(userId, wishlistId, cancellationToken);
        _ = await products.GetByIdAsync(productId, cancellationToken)
            ?? throw new NotFoundAppException();
        var itemId = await wishlists.GetItemIdAsync(wishlistId, productId, cancellationToken);
        if (itemId.HasValue)
            await wishlists.RestoreItemAsync(wishlistId, productId, userId, cancellationToken);
        else
            await wishlists.CreateItemAsync(idGenerator.NewId(), wishlistId, productId, userId, cancellationToken);
    }

    public async Task RemoveItemAsync(Guid userId, Guid wishlistId, Guid productId, CancellationToken cancellationToken = default)
    {
        _ = await GetByIdAsync(userId, wishlistId, cancellationToken);
        if (!await wishlists.SoftDeleteItemAsync(wishlistId, productId, userId, cancellationToken))
            throw new NotFoundAppException();
    }
}

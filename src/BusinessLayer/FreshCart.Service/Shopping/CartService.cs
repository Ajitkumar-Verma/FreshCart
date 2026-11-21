using FreshCart.Common.Exceptions;
using FreshCart.Common.Persistence;
using FreshCart.Common.Shopping;
using FreshCart.IRepository.Shopping;
using FreshCart.IService.Catalog;
using FreshCart.IService.Shopping;
using System.Data;

namespace FreshCart.Service.Shopping;

public sealed class CartService(
    ICartRepository carts,
    IProductService products,
    IIdGenerator idGenerator,
    IUnitOfWork unitOfWork) : ICartService
{
    public async Task<Cart> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        Cart cart;
        try
        {
            cart = await GetOrCreateAsync(userId, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(CancellationToken.None);
            throw;
        }
        return await LoadAsync(cart, cancellationToken);
    }

    public async Task<Cart> AddItemAsync(Guid userId, Guid productVariantId, CartItemRequest request, CancellationToken cancellationToken = default)
    {
        _ = await products.GetVariantForStoreAsync(productVariantId, cancellationToken);
        await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        Cart cart;
        try
        {
            cart = await GetOrCreateAsync(userId, cancellationToken);
            var item = await carts.GetItemByVariantAsync(cart.Id, productVariantId, cancellationToken);
            if (item is null)
                await carts.CreateItemAsync(idGenerator.NewId(), cart.Id, productVariantId, request.Quantity, userId, cancellationToken);
            else
                await carts.UpdateItemAsync(item.Id, item.IsDeleted ? request.Quantity : item.Quantity + request.Quantity, userId, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(CancellationToken.None);
            throw;
        }
        return await LoadAsync(cart, cancellationToken);
    }

    public async Task<Cart> UpdateItemAsync(Guid userId, Guid productVariantId, CartItemRequest request, CancellationToken cancellationToken = default)
    {
        var cart = await carts.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new NotFoundAppException();
        var item = await carts.GetItemByVariantAsync(cart.Id, productVariantId, cancellationToken);
        if (item is null || item.IsDeleted)
            throw new NotFoundAppException();

        await carts.UpdateItemAsync(item.Id, request.Quantity, userId, cancellationToken);
        return await LoadAsync(cart, cancellationToken);
    }

    public async Task<Cart> RemoveItemAsync(Guid userId, Guid productVariantId, CancellationToken cancellationToken = default)
    {
        var cart = await carts.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new NotFoundAppException();
        if (!await carts.SoftDeleteItemAsync(cart.Id, productVariantId, userId, cancellationToken))
            throw new NotFoundAppException();
        return await LoadAsync(cart, cancellationToken);
    }

    private async Task<Cart> GetOrCreateAsync(Guid userId, CancellationToken cancellationToken)
    {
        var cart = await carts.GetByUserIdAsync(userId, cancellationToken);
        if (cart is not null) return cart;
        cart = new Cart { Id = idGenerator.NewId(), UserId = userId };
        await carts.CreateAsync(cart.Id, userId, cancellationToken);
        return cart;
    }

    private async Task<Cart> LoadAsync(Cart cart, CancellationToken cancellationToken)
    {
        var items = await carts.GetItemsAsync(cart.Id, cancellationToken);
        cart.Items = items;
        cart.Subtotal = items.Sum(item => item.LineTotal);
        return cart;
    }
}

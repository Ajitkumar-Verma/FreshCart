using FreshCart.Common.Catalog;
using FreshCart.Common.Constants;
using FreshCart.Common.Exceptions;
using FreshCart.Common.Marketplace;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Catalog;
using FreshCart.IRepository.Marketplace;
using FreshCart.IService.Catalog;
using System.Data;

namespace FreshCart.Service.Catalog;

public sealed class ProductService(
    IProductRepository products,
    ICategoryRepository categories,
    IShopRepository shops,
    IIdGenerator idGenerator,
    IUnitOfWork unitOfWork) : IProductService
{
    public Task<IReadOnlyList<Product>> GetAvailableAsync(Guid? shopId = null, Guid? categoryId = null, string? search = null, CancellationToken cancellationToken = default)
    {
        if (shopId == Guid.Empty) throw new ValidationAppException(AppErrorCodes.ShopIdInvalid);
        if (categoryId == Guid.Empty) throw new ValidationAppException(AppErrorCodes.CategoryIdInvalid);
        return products.GetAvailableAsync(shopId, categoryId, search, cancellationToken);
    }

    public Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty) throw new ValidationAppException(AppErrorCodes.ProductIdInvalid);
        return products.GetByIdAsync(productId, cancellationToken);
    }

    public async Task<Product> GetManagedByIdAsync(Guid productId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty) throw new ValidationAppException(AppErrorCodes.ProductIdInvalid);
        var shopId = await products.GetShopIdAsync(productId, cancellationToken)
            ?? throw new NotFoundAppException();
        if (!await shops.CanManageAsync(shopId, actorUserId, cancellationToken))
            throw new ForbiddenAppException();
        return await products.GetForManagementAsync(productId, cancellationToken)
            ?? throw new NotFoundAppException();
    }

    public async Task<Product> CreateAsync(ProductCreateRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        if (request.ShopId == Guid.Empty) throw new ValidationAppException(AppErrorCodes.ShopIdInvalid);
        if (request.CategoryId == Guid.Empty) throw new ValidationAppException(AppErrorCodes.CategoryIdInvalid);
        if (!await shops.CanManageAsync(request.ShopId, actorUserId, cancellationToken))
            throw new ForbiddenAppException();
        if (!await categories.ExistsAsync(request.CategoryId, cancellationToken))
            throw new ValidationAppException(AppErrorCodes.CategoryIdInvalid);

        var productId = idGenerator.NewId();
        var variantId = idGenerator.NewId();
        var priceHistoryId = idGenerator.NewId();
        var name = request.Name.Trim();
        var sku = request.SKU.Trim().ToUpperInvariant();
        var unitName = request.UnitName.Trim();
        var currencyCode = request.CurrencyCode.Trim().ToUpperInvariant();
        var description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();

        var variant = new ProductVariant
        {
            Id = variantId,
            ProductId = productId,
            SKU = sku,
            UnitName = unitName,
            UnitQuantity = request.UnitQuantity,
            UnitPrice = request.UnitPrice,
            CurrencyCode = currencyCode,
            IsDefault = true,
            IsActive = true
        };

        await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            await products.CreateAsync(productId, request.ShopId, request.CategoryId, name, description, actorUserId, cancellationToken);
            await products.CreateVariantAsync(variant, actorUserId, cancellationToken);
            await products.CreatePriceHistoryAsync(priceHistoryId, variantId, null, request.UnitPrice, actorUserId, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(CancellationToken.None);
            throw;
        }

        return new Product
        {
            ProductId = productId,
            ProductVariantId = variantId,
            ShopId = request.ShopId,
            CategoryId = request.CategoryId,
            Name = name,
            Description = description,
            SKU = sku,
            UnitName = unitName,
            UnitQuantity = request.UnitQuantity,
            Price = request.UnitPrice,
            CurrencyCode = currencyCode,
            IsAvailable = false
        };
    }

    public async Task<Product> UpdateAsync(Guid productId, ProductUpdateRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty) throw new ValidationAppException(AppErrorCodes.ProductIdInvalid);
        var shopId = await products.GetShopIdAsync(productId, cancellationToken)
            ?? throw new NotFoundAppException();
        if (!await shops.CanManageAsync(shopId, actorUserId, cancellationToken))
            throw new ForbiddenAppException();
        if (!await categories.ExistsAsync(request.CategoryId, cancellationToken))
            throw new ValidationAppException(AppErrorCodes.CategoryIdInvalid);

        await products.UpdateAsync(
            productId,
            request.CategoryId,
            request.Name.Trim(),
            string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            request.IsActive,
            actorUserId,
            cancellationToken);
        return await products.GetForManagementAsync(productId, cancellationToken)
            ?? throw new NotFoundAppException();
    }

    public async Task DeleteAsync(Guid productId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty) throw new ValidationAppException(AppErrorCodes.ProductIdInvalid);
        var shopId = await products.GetShopIdAsync(productId, cancellationToken)
            ?? throw new NotFoundAppException();
        if (!await shops.CanManageAsync(shopId, actorUserId, cancellationToken))
            throw new ForbiddenAppException();
        if (!await products.SoftDeleteAsync(productId, actorUserId, cancellationToken))
            throw new NotFoundAppException();
    }

    public async Task<IReadOnlyList<ProductVariant>> GetVariantsAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty) throw new ValidationAppException(AppErrorCodes.ProductIdInvalid);
        if (await products.GetByIdAsync(productId, cancellationToken) is null)
            throw new NotFoundAppException();
        return await products.GetVariantsAsync(productId, cancellationToken);
    }

    public async Task<ProductVariant> GetVariantForManagementAsync(Guid variantId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        var variant = await products.GetVariantByIdAsync(variantId, cancellationToken)
            ?? throw new NotFoundAppException();
        var productId = await products.GetProductIdByVariantIdAsync(variantId, cancellationToken)
            ?? throw new NotFoundAppException();
        var shopId = await products.GetShopIdAsync(productId, cancellationToken)
            ?? throw new NotFoundAppException();
        if (!await shops.CanManageAsync(shopId, actorUserId, cancellationToken))
            throw new ForbiddenAppException();
        return variant;
    }

    public async Task<ProductVariant> GetVariantForStoreAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        var variant = await products.GetVariantByIdAsync(variantId, cancellationToken)
            ?? throw new NotFoundAppException();
        if (!variant.IsActive)
            throw new NotFoundAppException();
        var productId = await products.GetProductIdByVariantIdAsync(variantId, cancellationToken)
            ?? throw new NotFoundAppException();
        _ = await products.GetByIdAsync(productId, cancellationToken)
            ?? throw new NotFoundAppException();
        return variant;
    }

    public async Task<ProductVariant> CreateVariantAsync(Guid productId, ProductVariantRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        if (request.IsDefault && !request.IsActive)
            throw new ValidationAppException();
        var shopId = await products.GetShopIdAsync(productId, cancellationToken)
            ?? throw new NotFoundAppException();
        if (!await shops.CanManageAsync(shopId, actorUserId, cancellationToken))
            throw new ForbiddenAppException();

        var variant = MapVariant(request, productId, idGenerator.NewId());
        var priceHistoryId = idGenerator.NewId();
        await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            if (variant.IsDefault)
                await products.ClearDefaultVariantAsync(productId, cancellationToken);
            await products.CreateVariantAsync(variant, actorUserId, cancellationToken);
            await products.CreatePriceHistoryAsync(priceHistoryId, variant.Id, null, variant.UnitPrice, actorUserId, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return variant;
        }
        catch
        {
            await unitOfWork.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task<ProductVariant> UpdateVariantAsync(Guid variantId, ProductVariantRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        if (request.IsDefault && !request.IsActive)
            throw new ValidationAppException();
        var existing = await GetVariantForManagementAsync(variantId, actorUserId, cancellationToken);
        if (existing.IsDefault && !request.IsDefault)
            throw new ConflictAppException();

        var updated = MapVariant(request, existing.ProductId, variantId);
        updated.IsActive = request.IsActive;
        var priceHistoryId = idGenerator.NewId();
        await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            if (updated.IsDefault && !existing.IsDefault)
                await products.ClearDefaultVariantAsync(existing.ProductId, cancellationToken);
            await products.UpdateVariantAsync(updated, actorUserId, cancellationToken);
            if (existing.UnitPrice != updated.UnitPrice)
                await products.CreatePriceHistoryAsync(priceHistoryId, variantId, existing.UnitPrice, updated.UnitPrice, actorUserId, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return updated;
        }
        catch
        {
            await unitOfWork.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task DeleteVariantAsync(Guid variantId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        var variant = await GetVariantForManagementAsync(variantId, actorUserId, cancellationToken);
        if (variant.IsDefault)
            throw new ConflictAppException();
        if (!await products.SoftDeleteVariantAsync(variantId, actorUserId, cancellationToken))
            throw new NotFoundAppException();
    }

    private static ProductVariant MapVariant(ProductVariantRequest request, Guid productId, Guid id) => new()
    {
        Id = id,
        ProductId = productId,
        SKU = request.SKU.Trim().ToUpperInvariant(),
        UnitName = request.UnitName.Trim(),
        UnitQuantity = request.UnitQuantity,
        UnitPrice = request.UnitPrice,
        CurrencyCode = request.CurrencyCode.Trim().ToUpperInvariant(),
        IsDefault = request.IsDefault,
        IsActive = request.IsActive
    };
}


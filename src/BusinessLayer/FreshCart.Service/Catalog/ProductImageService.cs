using FreshCart.Common.Catalog;
using FreshCart.Common.Exceptions;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Catalog;
using FreshCart.IRepository.Marketplace;
using FreshCart.IService.Catalog;

namespace FreshCart.Service.Catalog;

public sealed class ProductImageService(
    IProductImageRepository images,
    IProductRepository products,
    IShopRepository shops,
    IIdGenerator idGenerator) : IProductImageService
{
    public async Task<IReadOnlyList<ProductImage>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        if (await products.GetByIdAsync(productId, cancellationToken) is null)
            throw new NotFoundAppException();
        return await images.GetByProductAsync(productId, cancellationToken);
    }

    public async Task<ProductImage> CreateAsync(Guid productId, ProductImageRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureProductAccessAsync(productId, actorUserId, cancellationToken);
        Validate(request);
        var image = Map(request, idGenerator.NewId(), productId);
        return await images.CreateAsync(image, actorUserId, cancellationToken);
    }

    public async Task<ProductImage> UpdateAsync(Guid productId, Guid imageId, ProductImageRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureProductAccessAsync(productId, actorUserId, cancellationToken);
        Validate(request);
        return await images.UpdateAsync(Map(request, imageId, productId), actorUserId, cancellationToken)
            ?? throw new NotFoundAppException();
    }

    public async Task DeleteAsync(Guid productId, Guid imageId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        await EnsureProductAccessAsync(productId, actorUserId, cancellationToken);
        if (!await images.SoftDeleteAsync(productId, imageId, actorUserId, cancellationToken))
            throw new NotFoundAppException();
    }

    private async Task EnsureProductAccessAsync(Guid productId, Guid actorUserId, CancellationToken cancellationToken)
    {
        var shopId = await products.GetShopIdAsync(productId, cancellationToken) ?? throw new NotFoundAppException();
        if (!await shops.CanManageAsync(shopId, actorUserId, cancellationToken))
            throw new ForbiddenAppException();
    }

    private static void Validate(ProductImageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ImageUrl) || request.SortOrder < 0)
            throw new ValidationAppException();
    }

    private static ProductImage Map(ProductImageRequest request, Guid id, Guid productId) => new()
    {
        Id = id,
        ProductId = productId,
        ImageUrl = request.ImageUrl.Trim(),
        AltText = request.AltText?.Trim(),
        SortOrder = request.SortOrder
    };
}

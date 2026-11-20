using FreshCart.Common.Catalog;
using FreshCart.IRepository.Catalog;
using FreshCart.IService.Catalog;

namespace FreshCart.Service.Catalog;

public sealed class ProductService(IProductRepository products) : IProductService
{
    public Task<IReadOnlyList<Product>> GetAvailableAsync(int? shopId = null, int? categoryId = null, string? search = null, CancellationToken cancellationToken = default)
    {
        if (shopId <= 0) throw new ArgumentOutOfRangeException(nameof(shopId));
        if (categoryId <= 0) throw new ArgumentOutOfRangeException(nameof(categoryId));
        return products.GetAvailableAsync(shopId, categoryId, search, cancellationToken);
    }

    public Task<Product?> GetByIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        if (productId <= 0) throw new ArgumentOutOfRangeException(nameof(productId));
        return products.GetByIdAsync(productId, cancellationToken);
    }
}

using FreshCart.Common.Catalog;
using FreshCart.Common.Exceptions;
using FreshCart.IRepository.Catalog;
using FreshCart.IService.Catalog;

namespace FreshCart.Service.Catalog;

public sealed class ProductService(IProductRepository products) : IProductService
{
    public Task<IReadOnlyList<Product>> GetAvailableAsync(Guid? shopId = null, Guid? categoryId = null, string? search = null, CancellationToken cancellationToken = default)
    {
        if (shopId == Guid.Empty) throw new ValidationAppException("Shop ID cannot be an empty GUID.");
        if (categoryId == Guid.Empty) throw new ValidationAppException("Category ID cannot be an empty GUID.");
        return products.GetAvailableAsync(shopId, categoryId, search, cancellationToken);
    }

    public Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty) throw new ValidationAppException("Product ID cannot be an empty GUID.");
        return products.GetByIdAsync(productId, cancellationToken);
    }
}

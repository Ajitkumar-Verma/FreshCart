using FreshCart.Common.Catalog;

namespace FreshCart.IService.Catalog;

public interface IProductService
{
    Task<IReadOnlyList<Product>> GetAvailableAsync(int? shopId = null, int? categoryId = null, string? search = null, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(int productId, CancellationToken cancellationToken = default);
}

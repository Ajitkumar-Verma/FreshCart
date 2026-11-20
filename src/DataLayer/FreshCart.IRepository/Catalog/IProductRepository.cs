using FreshCart.Common.Catalog;

namespace FreshCart.IRepository.Catalog;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAvailableAsync(int? shopId = null, int? categoryId = null, string? search = null, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(int productId, CancellationToken cancellationToken = default);
}

using FreshCart.Common.Catalog;

namespace FreshCart.IService.Catalog;

public interface IProductService
{
    /// <summary>Returns active products matching optional shop, category, and text filters.</summary>
    Task<IReadOnlyList<Product>> GetAvailableAsync(Guid? shopId = null, Guid? categoryId = null, string? search = null, CancellationToken cancellationToken = default);
    /// <summary>Returns one active product by its public GUID, or null when it does not exist.</summary>
    Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);
}

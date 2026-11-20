using FreshCart.Common.Catalog;

namespace FreshCart.IRepository.Catalog;

public interface IProductRepository
{
    /// <summary>Loads active catalog rows by calling the catalog listing stored procedure.</summary>
    Task<IReadOnlyList<Product>> GetAvailableAsync(Guid? shopId = null, Guid? categoryId = null, string? search = null, CancellationToken cancellationToken = default);

    /// <summary>Loads one active catalog row by GUID using the catalog detail stored procedure.</summary>
    Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);
}

using FreshCart.Common.Catalog;

namespace FreshCart.IService.Catalog;

public interface ICategoryService
{
    /// <summary>Returns active categories ordered for storefront display.</summary>
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns one active category by its identifier or raises not found.</summary>
    Task<Category> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);

    /// <summary>Creates an active category after checking its slug and optional parent.</summary>
    Task<Category> CreateAsync(CategoryRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Updates a category and its audit fields.</summary>
    Task<Category> UpdateAsync(Guid categoryId, CategoryRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes a category so historical product references remain intact.</summary>
    Task DeleteAsync(Guid categoryId, Guid actorUserId, CancellationToken cancellationToken = default);
}

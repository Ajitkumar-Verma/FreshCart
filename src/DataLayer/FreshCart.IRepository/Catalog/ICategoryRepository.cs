using FreshCart.Common.Catalog;

namespace FreshCart.IRepository.Catalog;

public interface ICategoryRepository
{
    /// <summary>Returns active categories in display order.</summary>
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Finds an active category by identifier.</summary>
    Task<Category?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);

    /// <summary>Finds a category by slug to detect duplicate category URLs.</summary>
    Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>Checks whether an active parent category exists.</summary>
    Task<bool> ExistsAsync(Guid categoryId, CancellationToken cancellationToken = default);

    /// <summary>Inserts one category and returns the persisted row.</summary>
    Task<Category> CreateAsync(Category category, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Updates one category and returns the persisted row.</summary>
    Task<Category?> UpdateAsync(Category category, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Marks one category as deleted.</summary>
    Task<bool> SoftDeleteAsync(Guid categoryId, Guid actorUserId, CancellationToken cancellationToken = default);
}

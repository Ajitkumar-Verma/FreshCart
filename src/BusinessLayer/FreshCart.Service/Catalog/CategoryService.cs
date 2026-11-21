using FreshCart.Common.Catalog;
using FreshCart.Common.Exceptions;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Catalog;
using FreshCart.IService.Catalog;

namespace FreshCart.Service.Catalog;

public sealed class CategoryService(ICategoryRepository categories, IIdGenerator idGenerator) : ICategoryService
{
    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await categories.GetAllAsync(cancellationToken);
    }

    public async Task<Category> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await categories.GetByIdAsync(categoryId, cancellationToken)
            ?? throw new NotFoundAppException();
    }

    public async Task<Category> CreateAsync(CategoryRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        var category = Map(request, idGenerator.NewId());
        await ValidateAsync(category, null, cancellationToken);
        return await categories.CreateAsync(category, actorUserId, cancellationToken);
    }

    public async Task<Category> UpdateAsync(Guid categoryId, CategoryRequest request, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        _ = await GetByIdAsync(categoryId, cancellationToken);
        var category = Map(request, categoryId);
        await ValidateAsync(category, categoryId, cancellationToken);
        return await categories.UpdateAsync(category, actorUserId, cancellationToken)
            ?? throw new NotFoundAppException();
    }

    public async Task DeleteAsync(Guid categoryId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        _ = await GetByIdAsync(categoryId, cancellationToken);
        if (!await categories.SoftDeleteAsync(categoryId, actorUserId, cancellationToken))
            throw new NotFoundAppException();
    }

    private async Task ValidateAsync(Category category, Guid? currentId, CancellationToken cancellationToken)
    {
        if (category.ParentCategoryId == category.Id || category.ParentCategoryId == currentId)
            throw new ValidationAppException();
        if (category.ParentCategoryId.HasValue &&
            !await categories.ExistsAsync(category.ParentCategoryId.Value, cancellationToken))
            throw new ValidationAppException();

        var existing = await categories.GetBySlugAsync(category.Slug, cancellationToken);
        if (existing is not null && existing.Id != currentId)
            throw new ConflictAppException();
    }

    private static Category Map(CategoryRequest request, Guid id) => new()
    {
        Id = id,
        Name = request.Name.Trim(),
        Slug = request.Slug.Trim().ToLowerInvariant(),
        ParentCategoryId = request.ParentCategoryId,
        IsActive = request.IsActive,
        SortOrder = request.SortOrder
    };
}

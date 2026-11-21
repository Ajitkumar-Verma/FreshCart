using Dapper;
using FreshCart.Common.Catalog;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Catalog;
using System.Data;

namespace FreshCart.Repository.Catalog;

public sealed class CategoryRepository(IUnitOfWork unitOfWork) : ICategoryRepository
{
    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.CategoryGetAll,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return (await connection.QueryAsync<Category>(command)).AsList();
            }, cancellationToken);
    }

    public async Task<Category?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.CategoryGetById,
                    parameters: new { CategoryId = categoryId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<Category>(command);
            }, cancellationToken);
    }

    public async Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.CategoryGetBySlug,
                    parameters: new { Slug = slug },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<Category>(command);
            }, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.CategoryExists,
                    parameters: new { CategoryId = categoryId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }

    public async Task<Category> CreateAsync(Category category, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.CategoryCreate,
                    parameters: new { category.Id, category.ParentCategoryId, category.Name, category.Slug, category.IsActive, category.SortOrder, CreatedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleAsync<Category>(command);
            }, cancellationToken);
    }

    public async Task<Category?> UpdateAsync(Category category, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.CategoryUpdate,
                    parameters: new { category.Id, category.ParentCategoryId, category.Name, category.Slug, category.IsActive, category.SortOrder, UpdatedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return connection.QuerySingleOrDefaultAsync<Category>(command);
            }, cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(Guid categoryId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        return await unitOfWork.WithConnectionAsync(
            async (connection, transaction) =>
            {
                var command = new CommandDefinition(
                    commandText: StoredProcedure.Catalog.CategorySoftDelete,
                    parameters: new { CategoryId = categoryId, DeletedBy = actorUserId },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken);
                return await connection.QuerySingleAsync<bool>(command);
            }, cancellationToken);
    }
}

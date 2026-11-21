using FreshCart.Common.Marketplace;

namespace FreshCart.IRepository.Marketplace;

public interface IShopBranchRepository
{
    /// <summary>Returns active branches of publicly approved shops.</summary>
    Task<IReadOnlyList<ShopBranch>> GetPublicAsync(Guid shopId, CancellationToken cancellationToken = default);

    /// <summary>Finds a non-deleted branch by shop and branch identifiers.</summary>
    Task<ShopBranch?> GetByIdAsync(Guid shopId, Guid branchId, CancellationToken cancellationToken = default);

    /// <summary>Inserts one shop branch row.</summary>
    Task<ShopBranch> CreateAsync(ShopBranch branch, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Updates branch details and returns the persisted branch.</summary>
    Task<ShopBranch?> UpdateAsync(ShopBranch branch, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes one branch.</summary>
    Task<bool> SoftDeleteAsync(Guid shopId, Guid branchId, Guid actorUserId, CancellationToken cancellationToken = default);
}

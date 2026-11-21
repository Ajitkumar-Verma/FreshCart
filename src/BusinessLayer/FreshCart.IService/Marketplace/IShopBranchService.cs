using FreshCart.Common.Marketplace;

namespace FreshCart.IService.Marketplace;

public interface IShopBranchService
{
    /// <summary>Lists active branches of an approved shop for storefront delivery selection.</summary>
    Task<IReadOnlyList<ShopBranch>> GetPublicAsync(Guid shopId, CancellationToken cancellationToken = default);

    /// <summary>Returns one branch after verifying the caller manages its shop.</summary>
    Task<ShopBranch> GetByIdAsync(Guid shopId, Guid branchId, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Creates a branch for a shop managed by the caller.</summary>
    Task<ShopBranch> CreateAsync(Guid shopId, ShopBranchRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Updates a branch after verifying shop ownership.</summary>
    Task<ShopBranch> UpdateAsync(Guid shopId, Guid branchId, ShopBranchRequest request, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes a branch while retaining its inventory and order history.</summary>
    Task DeleteAsync(Guid shopId, Guid branchId, Guid actorUserId, CancellationToken cancellationToken = default);
}

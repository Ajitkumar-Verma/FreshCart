using FreshCart.Common.Shopping;

namespace FreshCart.IRepository.Shopping;

public interface ICouponRepository
{
    /// <summary>Lists active, non-deleted coupons.</summary>
    Task<IReadOnlyList<Coupon>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Finds an active coupon by code for validation.</summary>
    Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>Finds a non-deleted coupon by identifier.</summary>
    Task<Coupon?> GetByIdAsync(Guid couponId, CancellationToken cancellationToken = default);

    /// <summary>Inserts one coupon row.</summary>
    Task<Coupon> CreateAsync(Coupon coupon, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Updates coupon fields.</summary>
    Task<Coupon?> UpdateAsync(Coupon coupon, Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes a coupon.</summary>
    Task<bool> SoftDeleteAsync(Guid couponId, Guid actorUserId, CancellationToken cancellationToken = default);
}

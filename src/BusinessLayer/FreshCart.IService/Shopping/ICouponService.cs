using FreshCart.Common.Shopping;

namespace FreshCart.IService.Shopping;

public interface ICouponService
{
    /// <summary>Lists active coupon records for platform administration.</summary>
    Task<IReadOnlyList<Coupon>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Validates one active coupon code against order subtotal and optional shop context.</summary>
    Task<Coupon> ValidateAsync(string code, decimal subtotal, Guid? shopId, CancellationToken cancellationToken = default);

    /// <summary>Creates a coupon after verifying platform or owning-shop permissions.</summary>
    Task<Coupon> CreateAsync(CouponRequest request, Guid actorUserId, bool isPlatformAdmin, CancellationToken cancellationToken = default);

    /// <summary>Updates a coupon after verifying platform or owning-shop permissions.</summary>
    Task<Coupon> UpdateAsync(Guid couponId, CouponRequest request, Guid actorUserId, bool isPlatformAdmin, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes a coupon after verifying platform or owning-shop permissions.</summary>
    Task DeleteAsync(Guid couponId, Guid actorUserId, bool isPlatformAdmin, CancellationToken cancellationToken = default);
}

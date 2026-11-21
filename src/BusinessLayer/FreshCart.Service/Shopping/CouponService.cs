using FreshCart.Common.Exceptions;
using FreshCart.Common.Persistence;
using FreshCart.Common.Shopping;
using FreshCart.IRepository.Marketplace;
using FreshCart.IRepository.Shopping;
using FreshCart.IService.Shopping;

namespace FreshCart.Service.Shopping;

public sealed class CouponService(ICouponRepository coupons, IShopRepository shops, IIdGenerator idGenerator) : ICouponService
{
    public async Task<IReadOnlyList<Coupon>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await coupons.GetAllAsync(cancellationToken);
    }

    public async Task<Coupon> ValidateAsync(string code, decimal subtotal, Guid? shopId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code) || subtotal < 0)
            throw new ValidationAppException();
        var coupon = await coupons.GetByCodeAsync(code.Trim().ToUpperInvariant(), cancellationToken) ?? throw new NotFoundAppException();
        var now = DateTime.UtcNow;
        if (!coupon.IsActive || coupon.StartsAt > now || coupon.EndsAt <= now || subtotal < coupon.MinimumOrder)
            throw new ConflictAppException();
        if (coupon.ShopId.HasValue && coupon.ShopId != shopId)
            throw new ConflictAppException();
        return coupon;
    }

    public async Task<Coupon> CreateAsync(CouponRequest request, Guid actorUserId, bool isPlatformAdmin, CancellationToken cancellationToken = default)
    {
        await EnsureCanManageAsync(request.ShopId, actorUserId, isPlatformAdmin, cancellationToken);
        ValidateRequest(request);
        var code = request.Code.Trim().ToUpperInvariant();
        if (await coupons.GetByCodeAsync(code, cancellationToken) is not null)
            throw new ConflictAppException();
        return await coupons.CreateAsync(Map(request, idGenerator.NewId(), code), actorUserId, cancellationToken);
    }

    public async Task<Coupon> UpdateAsync(Guid couponId, CouponRequest request, Guid actorUserId, bool isPlatformAdmin, CancellationToken cancellationToken = default)
    {
        var current = await coupons.GetByIdAsync(couponId, cancellationToken) ?? throw new NotFoundAppException();
        await EnsureCanManageAsync(current.ShopId, actorUserId, isPlatformAdmin, cancellationToken);
        await EnsureCanManageAsync(request.ShopId, actorUserId, isPlatformAdmin, cancellationToken);
        ValidateRequest(request);
        var code = request.Code.Trim().ToUpperInvariant();
        var existing = await coupons.GetByCodeAsync(code, cancellationToken);
        if (existing is not null && existing.Id != couponId)
            throw new ConflictAppException();
        return await coupons.UpdateAsync(Map(request, couponId, code), actorUserId, cancellationToken) ?? throw new NotFoundAppException();
    }

    public async Task DeleteAsync(Guid couponId, Guid actorUserId, bool isPlatformAdmin, CancellationToken cancellationToken = default)
    {
        var coupon = await coupons.GetByIdAsync(couponId, cancellationToken) ?? throw new NotFoundAppException();
        await EnsureCanManageAsync(coupon.ShopId, actorUserId, isPlatformAdmin, cancellationToken);
        if (!await coupons.SoftDeleteAsync(couponId, actorUserId, cancellationToken))
            throw new NotFoundAppException();
    }

    private async Task EnsureCanManageAsync(Guid? shopId, Guid actorUserId, bool isPlatformAdmin, CancellationToken cancellationToken)
    {
        if (isPlatformAdmin)
            return;
        if (!shopId.HasValue || !await shops.CanManageAsync(shopId.Value, actorUserId, cancellationToken))
            throw new ForbiddenAppException();
    }

    private static void ValidateRequest(CouponRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Code) || request.StartsAt == default || request.EndsAt <= request.StartsAt
            || request.DiscountValue <= 0 || request.MinimumOrder < 0 || request.DiscountType > 1
            || request.DiscountType == 0 && request.DiscountValue > 100)
            throw new ValidationAppException();
    }

    private static Coupon Map(CouponRequest request, Guid id, string code) => new()
    {
        Id = id,
        ShopId = request.ShopId,
        Code = code,
        DiscountType = request.DiscountType,
        DiscountValue = request.DiscountValue,
        MinimumOrder = request.MinimumOrder,
        StartsAt = request.StartsAt,
        EndsAt = request.EndsAt,
        UsageLimit = request.UsageLimit,
        IsActive = request.IsActive
    };
}

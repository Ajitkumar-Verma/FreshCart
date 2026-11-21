CREATE PROCEDURE dbo.Coupon_GetByCode
    @Code NVARCHAR(40)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT coupon.Id,
           coupon.ShopId,
           coupon.Code,
           coupon.DiscountType,
           coupon.DiscountValue,
           coupon.MinimumOrder,
           coupon.StartsAt,
           coupon.EndsAt,
           coupon.UsageLimit,
           coupon.IsActive
    FROM dbo.Coupons AS coupon
    WHERE coupon.Code = @Code
      AND coupon.IsDeleted = 0
      AND (coupon.UsageLimit IS NULL OR
          (SELECT COUNT_BIG(*) FROM dbo.CouponUsage AS couponUsage
           WHERE couponUsage.CouponId = coupon.Id AND couponUsage.IsDeleted = 0) < coupon.UsageLimit);
END;

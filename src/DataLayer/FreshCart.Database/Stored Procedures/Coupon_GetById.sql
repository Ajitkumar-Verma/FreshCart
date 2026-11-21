CREATE PROCEDURE dbo.Coupon_GetById
    @CouponId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ShopId, Code, DiscountType, DiscountValue, MinimumOrder, StartsAt, EndsAt, UsageLimit, IsActive
    FROM dbo.Coupons
    WHERE Id = @CouponId
      AND IsDeleted = 0;
END;

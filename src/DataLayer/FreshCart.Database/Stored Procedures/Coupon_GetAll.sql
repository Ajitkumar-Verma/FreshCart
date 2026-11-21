CREATE PROCEDURE dbo.Coupon_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ShopId, Code, DiscountType, DiscountValue, MinimumOrder, StartsAt, EndsAt, UsageLimit, IsActive
    FROM dbo.Coupons
    WHERE IsDeleted = 0
      AND IsActive = 1
    ORDER BY CreatedOn DESC;
END;

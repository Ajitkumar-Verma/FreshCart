CREATE PROCEDURE dbo.Coupon_SoftDelete
    @CouponId UNIQUEIDENTIFIER,
    @DeletedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Coupons
    SET IsDeleted = 1,
        IsActive = 0,
        DeletedBy = @DeletedBy,
        DeletedAt = SYSUTCDATETIME(),
        UpdatedBy = @DeletedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @CouponId
      AND IsDeleted = 0;

    SELECT CAST(CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS BIT);
END;

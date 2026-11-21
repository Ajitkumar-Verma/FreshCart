CREATE PROCEDURE dbo.Coupon_Update
    @Id UNIQUEIDENTIFIER,
    @ShopId UNIQUEIDENTIFIER = NULL,
    @Code NVARCHAR(40),
    @DiscountType TINYINT,
    @DiscountValue DECIMAL(10,2),
    @MinimumOrder DECIMAL(10,2),
    @StartsAt DATETIME2,
    @EndsAt DATETIME2,
    @UsageLimit INT = NULL,
    @IsActive BIT,
    @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Coupons
    SET ShopId = @ShopId,
        Code = @Code,
        DiscountType = @DiscountType,
        DiscountValue = @DiscountValue,
        MinimumOrder = @MinimumOrder,
        StartsAt = @StartsAt,
        EndsAt = @EndsAt,
        UsageLimit = @UsageLimit,
        IsActive = @IsActive,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @Id
      AND IsDeleted = 0;

    SELECT Id, ShopId, Code, DiscountType, DiscountValue, MinimumOrder, StartsAt, EndsAt, UsageLimit, IsActive
    FROM dbo.Coupons
    WHERE Id = @Id
      AND IsDeleted = 0;
END;

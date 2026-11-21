CREATE PROCEDURE dbo.Coupon_Create
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
    @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Coupons (Id, ShopId, Code, DiscountType, DiscountValue, MinimumOrder, StartsAt, EndsAt, UsageLimit, IsActive, CreatedBy)
    VALUES (@Id, @ShopId, @Code, @DiscountType, @DiscountValue, @MinimumOrder, @StartsAt, @EndsAt, @UsageLimit, @IsActive, @CreatedBy);

    SELECT Id, ShopId, Code, DiscountType, DiscountValue, MinimumOrder, StartsAt, EndsAt, UsageLimit, IsActive
    FROM dbo.Coupons
    WHERE Id = @Id;
END;

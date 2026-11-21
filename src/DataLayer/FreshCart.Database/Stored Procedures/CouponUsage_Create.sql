CREATE PROCEDURE dbo.CouponUsage_Create
    @Id UNIQUEIDENTIFIER,
    @CouponId UNIQUEIDENTIFIER,
    @OrderId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @DiscountAmount DECIMAL(10,2),
    @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.CouponUsage (Id, CouponId, OrderId, UserId, DiscountAmount, CreatedBy)
    VALUES (@Id, @CouponId, @OrderId, @UserId, @DiscountAmount, @CreatedBy);
END;

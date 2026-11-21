CREATE PROCEDURE dbo.CartItem_SoftDelete
    @CartId UNIQUEIDENTIFIER,
    @ProductVariantId UNIQUEIDENTIFIER,
    @DeletedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.CartItems
    SET IsDeleted = 1,
        DeletedBy = @DeletedBy,
        DeletedAt = SYSUTCDATETIME(),
        UpdatedBy = @DeletedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE CartId = @CartId
      AND ProductVariantId = @ProductVariantId
      AND IsDeleted = 0;

    SELECT CAST(CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS BIT);
END;

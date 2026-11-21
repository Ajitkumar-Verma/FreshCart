CREATE PROCEDURE dbo.WishlistItem_SoftDelete
    @WishlistId UNIQUEIDENTIFIER,
    @ProductId UNIQUEIDENTIFIER,
    @DeletedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.WishlistItems
    SET IsDeleted = 1,
        DeletedBy = @DeletedBy,
        DeletedAt = SYSUTCDATETIME(),
        UpdatedBy = @DeletedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE WishlistId = @WishlistId
      AND ProductId = @ProductId
      AND IsDeleted = 0;

    SELECT CAST(CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS BIT);
END;

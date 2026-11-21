CREATE PROCEDURE dbo.WishlistItem_Restore
    @WishlistId UNIQUEIDENTIFIER,
    @ProductId UNIQUEIDENTIFIER,
    @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.WishlistItems
    SET IsDeleted = 0,
        DeletedBy = NULL,
        DeletedAt = NULL,
        AddedAt = SYSUTCDATETIME(),
        UpdatedBy = @UpdatedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE WishlistId = @WishlistId
      AND ProductId = @ProductId;
END;

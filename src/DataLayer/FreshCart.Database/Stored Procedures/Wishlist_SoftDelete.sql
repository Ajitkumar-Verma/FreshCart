CREATE PROCEDURE dbo.Wishlist_SoftDelete
    @UserId UNIQUEIDENTIFIER,
    @WishlistId UNIQUEIDENTIFIER,
    @DeletedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Wishlists
    SET IsDeleted = 1,
        DeletedBy = @DeletedBy,
        DeletedAt = SYSUTCDATETIME(),
        UpdatedBy = @DeletedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @WishlistId
      AND UserId = @UserId
      AND IsDeleted = 0;

    SELECT CAST(CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS BIT);
END;

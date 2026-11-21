CREATE PROCEDURE dbo.Wishlist_Update
    @UserId UNIQUEIDENTIFIER,
    @WishlistId UNIQUEIDENTIFIER,
    @Name NVARCHAR(100),
    @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Wishlists
    SET Name = @Name,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @WishlistId
      AND UserId = @UserId
      AND IsDeleted = 0;
END;

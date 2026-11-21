CREATE PROCEDURE dbo.Wishlist_GetById
    @UserId UNIQUEIDENTIFIER,
    @WishlistId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name
    FROM dbo.Wishlists
    WHERE Id = @WishlistId
      AND UserId = @UserId
      AND IsDeleted = 0;
END;

CREATE PROCEDURE dbo.Wishlist_GetAll
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name
    FROM dbo.Wishlists
    WHERE UserId = @UserId
      AND IsDeleted = 0
    ORDER BY CreatedOn;
END;

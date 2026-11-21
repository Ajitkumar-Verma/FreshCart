CREATE PROCEDURE dbo.Wishlist_GetByName
    @UserId UNIQUEIDENTIFIER,
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name
    FROM dbo.Wishlists
    WHERE UserId = @UserId
      AND Name = @Name;
END;

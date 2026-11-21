CREATE PROCEDURE dbo.WishlistItem_GetId
    @WishlistId UNIQUEIDENTIFIER,
    @ProductId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id
    FROM dbo.WishlistItems
    WHERE WishlistId = @WishlistId
      AND ProductId = @ProductId;
END;

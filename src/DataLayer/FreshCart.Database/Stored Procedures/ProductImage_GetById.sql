CREATE PROCEDURE dbo.ProductImage_GetById
    @ProductId UNIQUEIDENTIFIER,
    @ImageId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, ProductId, ImageUrl, AltText, SortOrder
    FROM dbo.ProductImages
    WHERE Id = @ImageId AND ProductId = @ProductId AND IsDeleted = 0;
END;

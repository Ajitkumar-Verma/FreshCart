CREATE PROCEDURE dbo.ProductImage_GetByProduct
    @ProductId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, ProductId, ImageUrl, AltText, SortOrder
    FROM dbo.ProductImages
    WHERE ProductId = @ProductId AND IsDeleted = 0
    ORDER BY SortOrder, CreatedOn;
END;

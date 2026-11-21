CREATE PROCEDURE dbo.Product_GetShopId
    @ProductId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ShopId
    FROM dbo.Products
    WHERE Id = @ProductId
      AND IsDeleted = 0;
END;

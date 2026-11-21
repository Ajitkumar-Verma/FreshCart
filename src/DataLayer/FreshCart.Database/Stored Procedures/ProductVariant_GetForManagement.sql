CREATE PROCEDURE dbo.ProductVariant_GetForManagement
    @ProductId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ProductId, SKU, UnitName, UnitQuantity, UnitPrice, CurrencyCode, IsDefault, IsActive
    FROM dbo.ProductVariants
    WHERE ProductId = @ProductId
      AND IsDeleted = 0
    ORDER BY IsDefault DESC, UnitQuantity;
END;

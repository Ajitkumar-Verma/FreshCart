CREATE PROCEDURE dbo.ProductVariant_GetById
    @VariantId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ProductId, SKU, UnitName, UnitQuantity, UnitPrice, CurrencyCode, IsDefault, IsActive
    FROM dbo.ProductVariants
    WHERE Id = @VariantId
      AND IsDeleted = 0;
END;

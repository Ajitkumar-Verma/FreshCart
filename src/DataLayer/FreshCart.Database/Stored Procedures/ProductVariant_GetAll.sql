CREATE PROCEDURE dbo.ProductVariant_GetAll
    @ProductId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        variant.Id,
        variant.ProductId,
        variant.SKU,
        variant.UnitName,
        variant.UnitQuantity,
        variant.UnitPrice,
        variant.CurrencyCode,
        variant.IsDefault,
        variant.IsActive
    FROM dbo.ProductVariants AS variant
    INNER JOIN dbo.Products AS product
        ON product.Id = variant.ProductId
       AND product.IsActive = 1
       AND product.IsDeleted = 0
    INNER JOIN dbo.Shops AS shop
        ON shop.Id = product.ShopId
       AND shop.Status = 1
       AND shop.IsDeleted = 0
    WHERE variant.ProductId = @ProductId
      AND variant.IsActive = 1
      AND variant.IsDeleted = 0
    ORDER BY variant.IsDefault DESC, variant.UnitQuantity;
END;

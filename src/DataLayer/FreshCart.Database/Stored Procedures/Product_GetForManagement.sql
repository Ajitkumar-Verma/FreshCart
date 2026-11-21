CREATE PROCEDURE dbo.Product_GetForManagement
    @ProductId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        product.Id AS ProductId,
        variant.Id AS ProductVariantId,
        product.ShopId,
        product.CategoryId,
        product.Name,
        product.Description,
        variant.SKU,
        variant.UnitName,
        variant.UnitQuantity,
        variant.UnitPrice AS Price,
        variant.CurrencyCode,
        CAST(CASE WHEN EXISTS
        (
            SELECT 1
            FROM dbo.Inventory AS inventory
            WHERE inventory.ProductVariantId = variant.Id
              AND inventory.QuantityOnHand > 0
              AND inventory.IsDeleted = 0
        ) THEN 1 ELSE 0 END AS BIT) AS IsAvailable
    FROM dbo.Products AS product
    INNER JOIN dbo.ProductVariants AS variant
        ON variant.ProductId = product.Id
       AND variant.IsDefault = 1
       AND variant.IsDeleted = 0
    WHERE product.Id = @ProductId
      AND product.IsDeleted = 0;
END;

CREATE PROCEDURE dbo.Product_GetById
    @ProductId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        p.Id AS ProductId,
        p.ShopId,
        p.CategoryId,
        p.Name,
        p.Description,
        v.UnitPrice AS Price,
        v.CurrencyCode,
        CAST(
            CASE WHEN EXISTS
            (
                SELECT 1
                FROM dbo.Inventory AS i
                WHERE i.ProductVariantId = v.Id
                  AND i.QuantityOnHand > 0
                  AND i.IsDeleted = 0
            ) THEN 1 ELSE 0 END
            AS BIT
        ) AS IsAvailable
    FROM dbo.Products AS p
    INNER JOIN dbo.ProductVariants AS v
        ON v.ProductId = p.Id
       AND v.IsDefault = 1
       AND v.IsActive = 1
       AND v.IsDeleted = 0
    WHERE p.Id = @ProductId
      AND p.IsActive = 1
      AND p.IsDeleted = 0;
END;

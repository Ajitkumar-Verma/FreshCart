CREATE PROCEDURE dbo.Product_GetAvailable
    @ShopId UNIQUEIDENTIFIER = NULL,
    @CategoryId UNIQUEIDENTIFIER = NULL,
    @Search NVARCHAR(180) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
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
    WHERE p.IsActive = 1
      AND p.IsDeleted = 0
      AND (@ShopId IS NULL OR p.ShopId = @ShopId)
      AND (@CategoryId IS NULL OR p.CategoryId = @CategoryId)
      AND (@Search IS NULL OR p.Name LIKE N'%' + @Search + N'%')
    ORDER BY p.Name;
END;

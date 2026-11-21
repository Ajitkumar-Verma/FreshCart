CREATE PROCEDURE dbo.CartItem_GetAll
    @CartId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        item.Id,
        item.ProductVariantId,
        product.ShopId,
        product.Name AS ProductName,
        variant.SKU,
        variant.UnitName,
        variant.UnitQuantity,
        variant.UnitPrice,
        variant.CurrencyCode,
        item.Quantity,
        item.Quantity * variant.UnitPrice AS LineTotal,
        CAST(CASE WHEN product.IsActive = 1
                       AND variant.IsActive = 1
                       AND shop.Status = 1
                       AND EXISTS
                       (
                           SELECT 1
                           FROM dbo.Inventory AS inventory
                           WHERE inventory.ProductVariantId = variant.Id
                             AND inventory.QuantityOnHand >= item.Quantity
                             AND inventory.IsDeleted = 0
                       )
                  THEN 1 ELSE 0 END AS BIT) AS IsAvailable,
        item.IsDeleted
    FROM dbo.CartItems AS item
    INNER JOIN dbo.ProductVariants AS variant
        ON variant.Id = item.ProductVariantId
    INNER JOIN dbo.Products AS product
        ON product.Id = variant.ProductId
    INNER JOIN dbo.Shops AS shop
        ON shop.Id = product.ShopId
    WHERE item.CartId = @CartId
      AND item.IsDeleted = 0
      AND variant.IsDeleted = 0
      AND product.IsDeleted = 0
      AND shop.IsDeleted = 0
    ORDER BY item.CreatedOn;
END;

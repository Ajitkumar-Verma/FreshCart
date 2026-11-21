CREATE PROCEDURE dbo.WishlistItem_GetAll
    @WishlistId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        item.Id,
        product.Id AS ProductId,
        product.Name AS ProductName,
        product.ShopId,
        variant.UnitPrice AS Price,
        variant.CurrencyCode
    FROM dbo.WishlistItems AS item
    INNER JOIN dbo.Products AS product
        ON product.Id = item.ProductId
       AND product.IsActive = 1
       AND product.IsDeleted = 0
    INNER JOIN dbo.Shops AS shop
        ON shop.Id = product.ShopId
       AND shop.Status = 1
       AND shop.IsDeleted = 0
    INNER JOIN dbo.ProductVariants AS variant
        ON variant.ProductId = product.Id
       AND variant.IsDefault = 1
       AND variant.IsActive = 1
       AND variant.IsDeleted = 0
    WHERE item.WishlistId = @WishlistId
      AND item.IsDeleted = 0
    ORDER BY item.AddedAt DESC;
END;

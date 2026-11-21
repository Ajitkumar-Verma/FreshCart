CREATE PROCEDURE dbo.Inventory_GetById
    @InventoryId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT inventory.Id,
           inventory.ShopBranchId,
           inventory.ProductVariantId,
           product.Name AS ProductName,
           variant.UnitName AS VariantName,
           inventory.QuantityOnHand,
           inventory.ReorderLevel
    FROM dbo.Inventory AS inventory
    INNER JOIN dbo.ProductVariants AS variant ON variant.Id = inventory.ProductVariantId
    INNER JOIN dbo.Products AS product ON product.Id = variant.ProductId
    WHERE inventory.Id = @InventoryId
      AND inventory.IsDeleted = 0
      AND variant.IsDeleted = 0
      AND product.IsDeleted = 0;
END;

CREATE PROCEDURE dbo.Product_GetAvailable
    @ShopId int = NULL, @CategoryId int = NULL, @Search nvarchar(180) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.ProductId, p.ShopId, p.CategoryId, p.Name, p.Description, v.UnitPrice, v.CurrencyCode,
           CAST(CASE WHEN EXISTS (SELECT 1 FROM dbo.Inventory i WHERE i.ProductVariantId = v.ProductVariantId AND i.QuantityOnHand > 0) THEN 1 ELSE 0 END AS bit) AS IsAvailable
    FROM dbo.Products p
    INNER JOIN dbo.ProductVariants v ON v.ProductId = p.ProductId AND v.IsDefault = 1 AND v.IsActive = 1
    WHERE p.IsActive = 1 AND (@ShopId IS NULL OR p.ShopId = @ShopId)
      AND (@CategoryId IS NULL OR p.CategoryId = @CategoryId)
      AND (@Search IS NULL OR p.Name LIKE '%' + @Search + '%')
    ORDER BY p.Name;
END;
GO
CREATE PROCEDURE dbo.Product_GetById @ProductId int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP (1) p.ProductId, p.ShopId, p.CategoryId, p.Name, p.Description, v.UnitPrice, v.CurrencyCode,
           CAST(CASE WHEN EXISTS (SELECT 1 FROM dbo.Inventory i WHERE i.ProductVariantId = v.ProductVariantId AND i.QuantityOnHand > 0) THEN 1 ELSE 0 END AS bit) AS IsAvailable
    FROM dbo.Products p
    INNER JOIN dbo.ProductVariants v ON v.ProductId = p.ProductId AND v.IsDefault = 1 AND v.IsActive = 1
    WHERE p.ProductId = @ProductId AND p.IsActive = 1;
END;
GO
CREATE PROCEDURE dbo.Inventory_Adjust
    @InventoryId bigint, @QuantityChange decimal(12,3), @Reason nvarchar(40), @Notes nvarchar(500), @ChangedByUserId nvarchar(450)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;
    UPDATE dbo.Inventory WITH (UPDLOCK, ROWLOCK)
      SET QuantityOnHand = QuantityOnHand + @QuantityChange, UpdatedAt = SYSUTCDATETIME()
      WHERE InventoryId = @InventoryId AND QuantityOnHand + @QuantityChange >= 0;
    IF @@ROWCOUNT <> 1 BEGIN ROLLBACK; THROW 50001, 'Inventory record not found or adjustment would make stock negative.', 1; END;
    INSERT dbo.StockMovements(InventoryId, QuantityChange, Reason, Notes, ChangedByUserId)
      VALUES(@InventoryId, @QuantityChange, @Reason, @Notes, @ChangedByUserId);
    COMMIT;
END;

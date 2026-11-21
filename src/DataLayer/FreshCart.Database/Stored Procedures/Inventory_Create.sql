CREATE PROCEDURE dbo.Inventory_Create
    @Id UNIQUEIDENTIFIER,
    @ShopBranchId UNIQUEIDENTIFIER,
    @ProductVariantId UNIQUEIDENTIFIER,
    @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Inventory (Id, ShopBranchId, ProductVariantId, QuantityOnHand, CreatedBy)
    VALUES (@Id, @ShopBranchId, @ProductVariantId, 0, @CreatedBy);

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
    WHERE inventory.Id = @Id;
END;

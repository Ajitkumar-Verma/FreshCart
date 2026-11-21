CREATE PROCEDURE dbo.Inventory_UpdateQuantity
    @InventoryId UNIQUEIDENTIFIER,
    @QuantityChange DECIMAL(12,3),
    @ChangedByUserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Inventory
    SET QuantityOnHand = QuantityOnHand + @QuantityChange,
        UpdatedOn = SYSUTCDATETIME(),
        UpdatedBy = @ChangedByUserId
    WHERE Id = @InventoryId
      AND IsDeleted = 0
      AND QuantityOnHand + @QuantityChange >= 0;

    SELECT CAST(CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS BIT);
END;

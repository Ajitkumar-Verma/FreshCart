CREATE PROCEDURE dbo.DeliverySlot_SoftDelete
    @ShopBranchId UNIQUEIDENTIFIER, @SlotId UNIQUEIDENTIFIER, @DeletedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.DeliverySlots SET IsDeleted = 1, IsActive = 0, DeletedBy = @DeletedBy,
        DeletedAt = SYSUTCDATETIME(), UpdatedBy = @DeletedBy, UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @SlotId AND ShopBranchId = @ShopBranchId AND IsDeleted = 0;
    SELECT CAST(CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS BIT);
END;

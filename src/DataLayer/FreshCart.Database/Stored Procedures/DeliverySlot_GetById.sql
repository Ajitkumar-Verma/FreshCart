CREATE PROCEDURE dbo.DeliverySlot_GetById
    @ShopBranchId UNIQUEIDENTIFIER,
    @SlotId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, ShopBranchId, StartsAt, EndsAt, Capacity, IsActive
    FROM dbo.DeliverySlots
    WHERE Id = @SlotId AND ShopBranchId = @ShopBranchId AND IsDeleted = 0;
END;

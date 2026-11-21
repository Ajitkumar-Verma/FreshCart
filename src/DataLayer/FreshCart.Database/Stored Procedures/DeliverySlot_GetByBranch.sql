CREATE PROCEDURE dbo.DeliverySlot_GetByBranch
    @ShopBranchId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, ShopBranchId, StartsAt, EndsAt, Capacity, IsActive
    FROM dbo.DeliverySlots
    WHERE ShopBranchId = @ShopBranchId AND IsDeleted = 0
    ORDER BY StartsAt;
END;

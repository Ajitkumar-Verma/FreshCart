CREATE PROCEDURE dbo.DeliverySlot_Update
    @Id UNIQUEIDENTIFIER, @ShopBranchId UNIQUEIDENTIFIER, @StartsAt TIME, @EndsAt TIME,
    @Capacity INT, @IsActive BIT, @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.DeliverySlots SET StartsAt = @StartsAt, EndsAt = @EndsAt, Capacity = @Capacity,
        IsActive = @IsActive, UpdatedBy = @UpdatedBy, UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @Id AND ShopBranchId = @ShopBranchId AND IsDeleted = 0;
    SELECT Id, ShopBranchId, StartsAt, EndsAt, Capacity, IsActive
    FROM dbo.DeliverySlots WHERE Id = @Id AND ShopBranchId = @ShopBranchId AND IsDeleted = 0;
END;

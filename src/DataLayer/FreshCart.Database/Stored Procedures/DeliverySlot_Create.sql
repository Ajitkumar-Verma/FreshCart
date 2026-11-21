CREATE PROCEDURE dbo.DeliverySlot_Create
    @Id UNIQUEIDENTIFIER, @ShopBranchId UNIQUEIDENTIFIER, @StartsAt TIME, @EndsAt TIME,
    @Capacity INT, @IsActive BIT, @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.DeliverySlots (Id, ShopBranchId, StartsAt, EndsAt, Capacity, IsActive, CreatedBy)
    VALUES (@Id, @ShopBranchId, @StartsAt, @EndsAt, @Capacity, @IsActive, @CreatedBy);
    SELECT Id, ShopBranchId, StartsAt, EndsAt, Capacity, IsActive FROM dbo.DeliverySlots WHERE Id = @Id;
END;

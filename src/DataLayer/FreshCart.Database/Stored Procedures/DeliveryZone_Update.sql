CREATE PROCEDURE dbo.DeliveryZone_Update
    @Id UNIQUEIDENTIFIER, @ShopBranchId UNIQUEIDENTIFIER, @Name NVARCHAR(120), @PostalCode NVARCHAR(20),
    @DeliveryFee DECIMAL(10,2), @MinimumOrder DECIMAL(10,2), @IsActive BIT, @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.DeliveryZones SET Name = @Name, PostalCode = @PostalCode, DeliveryFee = @DeliveryFee,
        MinimumOrder = @MinimumOrder, IsActive = @IsActive, UpdatedBy = @UpdatedBy, UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @Id AND ShopBranchId = @ShopBranchId AND IsDeleted = 0;
    SELECT Id, ShopBranchId, Name, PostalCode, DeliveryFee, MinimumOrder, IsActive
    FROM dbo.DeliveryZones WHERE Id = @Id AND ShopBranchId = @ShopBranchId AND IsDeleted = 0;
END;

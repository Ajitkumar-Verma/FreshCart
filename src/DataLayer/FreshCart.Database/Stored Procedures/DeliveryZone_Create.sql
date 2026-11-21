CREATE PROCEDURE dbo.DeliveryZone_Create
    @Id UNIQUEIDENTIFIER, @ShopBranchId UNIQUEIDENTIFIER, @Name NVARCHAR(120), @PostalCode NVARCHAR(20),
    @DeliveryFee DECIMAL(10,2), @MinimumOrder DECIMAL(10,2), @IsActive BIT, @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.DeliveryZones (Id, ShopBranchId, Name, PostalCode, DeliveryFee, MinimumOrder, IsActive, CreatedBy)
    VALUES (@Id, @ShopBranchId, @Name, @PostalCode, @DeliveryFee, @MinimumOrder, @IsActive, @CreatedBy);
    SELECT Id, ShopBranchId, Name, PostalCode, DeliveryFee, MinimumOrder, IsActive FROM dbo.DeliveryZones WHERE Id = @Id;
END;

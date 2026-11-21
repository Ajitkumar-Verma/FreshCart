CREATE PROCEDURE dbo.DeliveryZone_GetById
    @ShopBranchId UNIQUEIDENTIFIER,
    @ZoneId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, ShopBranchId, Name, PostalCode, DeliveryFee, MinimumOrder, IsActive
    FROM dbo.DeliveryZones
    WHERE Id = @ZoneId AND ShopBranchId = @ShopBranchId AND IsDeleted = 0;
END;

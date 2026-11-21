CREATE PROCEDURE dbo.DeliveryZone_GetByBranch
    @ShopBranchId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, ShopBranchId, Name, PostalCode, DeliveryFee, MinimumOrder, IsActive
    FROM dbo.DeliveryZones
    WHERE ShopBranchId = @ShopBranchId AND IsDeleted = 0
    ORDER BY PostalCode, Name;
END;

CREATE PROCEDURE dbo.DeliveryZone_GetByPostalCode
    @PostalCode NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT zone.Id, zone.ShopBranchId, zone.Name, zone.PostalCode, zone.DeliveryFee, zone.MinimumOrder, zone.IsActive
    FROM dbo.DeliveryZones AS zone
    INNER JOIN dbo.ShopBranches AS branch ON branch.Id = zone.ShopBranchId
    INNER JOIN dbo.Shops AS shop ON shop.Id = branch.ShopId
    WHERE zone.PostalCode = @PostalCode AND zone.IsActive = 1 AND zone.IsDeleted = 0
      AND branch.IsActive = 1 AND branch.IsDeleted = 0 AND shop.Status = 1 AND shop.IsDeleted = 0
    ORDER BY zone.DeliveryFee, shop.Name, branch.Name;
END;

CREATE PROCEDURE dbo.ShopBranch_GetPublic
    @ShopId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT branch.Id, branch.ShopId, branch.Name, branch.AddressLine, branch.City, branch.PostalCode,
           branch.Latitude, branch.Longitude, branch.IsActive
    FROM dbo.ShopBranches AS branch
    INNER JOIN dbo.Shops AS shop
        ON shop.Id = branch.ShopId
       AND shop.Status = 1
       AND shop.IsDeleted = 0
    WHERE branch.ShopId = @ShopId
      AND branch.IsActive = 1
      AND branch.IsDeleted = 0
    ORDER BY branch.Name;
END;

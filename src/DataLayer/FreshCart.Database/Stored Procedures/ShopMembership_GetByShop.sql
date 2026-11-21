CREATE PROCEDURE dbo.ShopMembership_GetByShop
    @ShopId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ShopId, UserId, RoleName, IsActive
    FROM dbo.ShopMemberships
    WHERE ShopId = @ShopId
      AND IsActive = 1
      AND IsDeleted = 0
    ORDER BY CreatedOn;
END;

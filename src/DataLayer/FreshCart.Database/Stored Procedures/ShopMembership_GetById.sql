CREATE PROCEDURE dbo.ShopMembership_GetById
    @ShopId UNIQUEIDENTIFIER,
    @MembershipId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ShopId, UserId, RoleName, IsActive
    FROM dbo.ShopMemberships
    WHERE Id = @MembershipId
      AND ShopId = @ShopId
      AND IsDeleted = 0;
END;

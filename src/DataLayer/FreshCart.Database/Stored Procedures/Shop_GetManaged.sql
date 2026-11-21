CREATE PROCEDURE dbo.Shop_GetManaged
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT shop.Id, shop.Name, shop.Slug, shop.Description, shop.OwnerUserId, shop.Status, shop.PlatformFeePercent
    FROM dbo.Shops AS shop
    WHERE shop.IsDeleted = 0
      AND
      (
          shop.OwnerUserId = @UserId
          OR EXISTS
          (
              SELECT 1
              FROM dbo.ShopMemberships AS membership
              WHERE membership.ShopId = shop.Id
                AND membership.UserId = @UserId
                AND membership.IsActive = 1
                AND membership.IsDeleted = 0
          )
          OR EXISTS
          (
              SELECT 1
              FROM dbo.AspNetUserRoles AS userRole
              INNER JOIN dbo.AspNetRoles AS role
                  ON role.Id = userRole.RoleId
              WHERE userRole.UserId = @UserId
                AND userRole.IsDeleted = 0
                AND role.NormalizedName = N'PLATFORMADMIN'
                AND role.IsDeleted = 0
          )
      )
    ORDER BY shop.Name;
END;

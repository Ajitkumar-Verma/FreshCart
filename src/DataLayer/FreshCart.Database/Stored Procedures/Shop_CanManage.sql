CREATE PROCEDURE dbo.Shop_CanManage
    @ShopId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(CASE WHEN EXISTS
    (
        SELECT 1
        FROM dbo.Shops AS shop
        WHERE shop.Id = @ShopId
          AND shop.IsDeleted = 0
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
                    AND membership.RoleName = N'Manager'
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
    ) THEN 1 ELSE 0 END AS BIT);
END;

CREATE PROCEDURE dbo.Auth_UserGetActiveRoleUserCount
    @RoleId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*)
    FROM dbo.AspNetUserRoles AS userRole
    INNER JOIN dbo.AspNetUsers AS [user]
        ON [user].Id = userRole.UserId
       AND [user].IsDeleted = 0
    WHERE userRole.RoleId = @RoleId
      AND userRole.IsDeleted = 0;
END;

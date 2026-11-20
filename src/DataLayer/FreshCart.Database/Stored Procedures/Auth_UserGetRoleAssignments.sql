CREATE PROCEDURE dbo.Auth_UserGetRoleAssignments
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        userRole.UserId,
        role.Name AS RoleName
    FROM dbo.AspNetUserRoles AS userRole
    INNER JOIN dbo.AspNetUsers AS [user]
        ON [user].Id = userRole.UserId
       AND [user].IsDeleted = 0
    INNER JOIN dbo.AspNetRoles AS role
        ON role.Id = userRole.RoleId
       AND role.IsDeleted = 0
    WHERE userRole.IsDeleted = 0
    ORDER BY userRole.UserId, role.Name;
END;

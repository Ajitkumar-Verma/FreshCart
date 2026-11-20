CREATE PROCEDURE dbo.Auth_UserGetRoles
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.Name
    FROM dbo.AspNetUserRoles AS ur
    INNER JOIN dbo.AspNetRoles AS r
        ON r.Id = ur.RoleId
       AND r.IsDeleted = 0
    INNER JOIN dbo.AspNetUsers AS u
        ON u.Id = ur.UserId
       AND u.IsDeleted = 0
    WHERE ur.UserId = @UserId
      AND ur.IsDeleted = 0
      AND r.Name IS NOT NULL
    ORDER BY r.Name;
END;

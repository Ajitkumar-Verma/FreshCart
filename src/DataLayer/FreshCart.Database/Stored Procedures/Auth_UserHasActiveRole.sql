CREATE PROCEDURE dbo.Auth_UserHasActiveRole
    @RoleId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST
    (
        CASE WHEN EXISTS
        (
            SELECT 1
            FROM dbo.AspNetUserRoles AS UserRole
            INNER JOIN dbo.AspNetUsers AS [User]
                ON [User].Id = UserRole.UserId
            WHERE UserRole.RoleId = @RoleId
              AND UserRole.IsDeleted = 0
              AND [User].IsDeleted = 0
        ) THEN 1 ELSE 0 END
        AS BIT
    ) AS HasActiveUser;
END;

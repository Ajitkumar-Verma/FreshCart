CREATE PROCEDURE dbo.Auth_UserRoleCreate
    @UserId UNIQUEIDENTIFIER,
    @RoleId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT dbo.AspNetUserRoles(UserId, RoleId)
    VALUES (@UserId, @RoleId);
END;

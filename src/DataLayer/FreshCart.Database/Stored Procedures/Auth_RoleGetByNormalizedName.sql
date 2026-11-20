CREATE PROCEDURE dbo.Auth_RoleGetByNormalizedName
    @NormalizedName NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id
    FROM dbo.AspNetRoles
    WHERE NormalizedName = @NormalizedName
      AND IsDeleted = 0;
END;

CREATE PROCEDURE dbo.Auth_UserGetByEmail
    @NormalizedEmail NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        Id,
        Email,
        NormalizedEmail,
        UserName,
        NormalizedUserName,
        DisplayName,
        PhoneNumber,
        PasswordHash
    FROM dbo.AspNetUsers
    WHERE NormalizedEmail = @NormalizedEmail
      AND IsDeleted = 0;
END;

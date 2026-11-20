CREATE PROCEDURE dbo.Auth_UserCreate
    @Id UNIQUEIDENTIFIER,
    @Email NVARCHAR(256),
    @NormalizedEmail NVARCHAR(256),
    @UserName NVARCHAR(256),
    @NormalizedUserName NVARCHAR(256),
    @DisplayName NVARCHAR(160),
    @PhoneNumber NVARCHAR(30) = NULL,
    @PasswordHash NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT dbo.AspNetUsers
    (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail,
        EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
        PhoneNumber, DisplayName
    )
    VALUES
    (
        @Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail,
        0, @PasswordHash, CONVERT(NVARCHAR(36), NEWID()), CONVERT(NVARCHAR(36), NEWID()),
        @PhoneNumber, @DisplayName
    );
END;

CREATE PROCEDURE dbo.Auth_UserCreate
    @Id UNIQUEIDENTIFIER,
    @Email NVARCHAR(256),
    @NormalizedEmail NVARCHAR(256),
    @UserName NVARCHAR(256),
    @NormalizedUserName NVARCHAR(256),
    @DisplayName NVARCHAR(160),
    @PhoneNumber NVARCHAR(30) = NULL,
    @PasswordHash NVARCHAR(MAX),
    @RoleName NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.AspNetUsers WITH (UPDLOCK, HOLDLOCK)
        WHERE NormalizedEmail = @NormalizedEmail
          AND IsDeleted = 0
    )
    BEGIN
        COMMIT TRANSACTION;
        SELECT CAST(NULL AS UNIQUEIDENTIFIER) AS UserId, CAST(1 AS BIT) AS AlreadyExists;
        RETURN;
    END;

    DECLARE @RoleId UNIQUEIDENTIFIER;
    SELECT TOP (1) @RoleId = Id
    FROM dbo.AspNetRoles
    WHERE UPPER(Name) = UPPER(@RoleName)
      AND IsDeleted = 0;

    IF @RoleId IS NULL
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 51001, 'Requested account role is not configured.', 1;
    END;

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

    INSERT dbo.AspNetUserRoles(UserId, RoleId)
    VALUES (@Id, @RoleId);

    COMMIT TRANSACTION;
    SELECT @Id AS UserId, CAST(0 AS BIT) AS AlreadyExists;
END;

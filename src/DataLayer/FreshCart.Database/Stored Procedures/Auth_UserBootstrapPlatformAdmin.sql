CREATE PROCEDURE dbo.Auth_UserBootstrapPlatformAdmin
    @Id UNIQUEIDENTIFIER,
    @Email NVARCHAR(256),
    @NormalizedEmail NVARCHAR(256),
    @UserName NVARCHAR(256),
    @NormalizedUserName NVARCHAR(256),
    @DisplayName NVARCHAR(160),
    @PhoneNumber NVARCHAR(30) = NULL,
    @PasswordHash NVARCHAR(MAX),
    @RoleName NVARCHAR(256),
    @NormalizedRoleName NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    DECLARE @LockResult INT;
    EXEC @LockResult = sys.sp_getapplock
        @Resource = N'FreshCart.PlatformAdminBootstrap',
        @LockMode = N'Exclusive',
        @LockOwner = N'Transaction',
        @LockTimeout = 10000;

    IF @LockResult < 0
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 51002, 'Could not acquire platform-admin bootstrap lock.', 1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.AspNetUserRoles AS ur
        INNER JOIN dbo.AspNetRoles AS r ON r.Id = ur.RoleId
        INNER JOIN dbo.AspNetUsers AS u ON u.Id = ur.UserId
        WHERE UPPER(r.Name) = UPPER(@RoleName)
          AND r.IsDeleted = 0
          AND u.IsDeleted = 0
          AND ur.IsDeleted = 0
    )
    BEGIN
        COMMIT TRANSACTION;
        SELECT CAST(NULL AS UNIQUEIDENTIFIER) AS UserId, CAST(1 AS BIT) AS AlreadyExists;
        RETURN;
    END;

    IF EXISTS (SELECT 1 FROM dbo.AspNetUsers WITH (UPDLOCK, HOLDLOCK) WHERE NormalizedEmail = @NormalizedEmail AND IsDeleted = 0)
    BEGIN
        COMMIT TRANSACTION;
        SELECT CAST(NULL AS UNIQUEIDENTIFIER) AS UserId, CAST(1 AS BIT) AS AlreadyExists;
        RETURN;
    END;

    DECLARE @RoleId UNIQUEIDENTIFIER;
    SELECT TOP (1) @RoleId = Id FROM dbo.AspNetRoles WHERE NormalizedName = @NormalizedRoleName AND IsDeleted = 0;
    IF @RoleId IS NULL
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 51001, 'PlatformAdmin role is not configured.', 1;
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



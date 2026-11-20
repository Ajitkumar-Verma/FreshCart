CREATE PROCEDURE dbo.Auth_UserUpdate
    @UserId UNIQUEIDENTIFIER,
    @Email NVARCHAR(256),
    @NormalizedEmail NVARCHAR(256),
    @UserName NVARCHAR(256),
    @NormalizedUserName NVARCHAR(256),
    @DisplayName NVARCHAR(160),
    @PhoneNumber NVARCHAR(30) = NULL,
    @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.AspNetUsers
    SET Email = @Email,
        NormalizedEmail = @NormalizedEmail,
        UserName = @UserName,
        NormalizedUserName = @NormalizedUserName,
        DisplayName = @DisplayName,
        PhoneNumber = @PhoneNumber,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @UserId
      AND IsDeleted = 0;
END;

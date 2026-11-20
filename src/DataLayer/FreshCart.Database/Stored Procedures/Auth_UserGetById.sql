CREATE PROCEDURE dbo.Auth_UserGetById
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Email,
        DisplayName,
        PhoneNumber,
        CreatedOn,
        UpdatedOn
    FROM dbo.AspNetUsers
    WHERE Id = @UserId
      AND IsDeleted = 0;
END;

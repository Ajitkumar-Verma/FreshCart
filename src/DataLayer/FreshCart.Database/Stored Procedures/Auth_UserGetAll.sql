CREATE PROCEDURE dbo.Auth_UserGetAll
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
    WHERE IsDeleted = 0
    ORDER BY CreatedOn DESC, Id;
END;

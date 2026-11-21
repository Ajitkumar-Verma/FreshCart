CREATE PROCEDURE dbo.Address_GetAll
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, [Label], RecipientName, Phone, AddressLine, City, PostalCode, IsDefault
    FROM dbo.Addresses
    WHERE UserId = @UserId
      AND IsDeleted = 0
    ORDER BY IsDefault DESC, CreatedOn DESC;
END;

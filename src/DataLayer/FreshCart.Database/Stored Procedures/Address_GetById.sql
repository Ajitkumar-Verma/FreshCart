CREATE PROCEDURE dbo.Address_GetById
    @UserId UNIQUEIDENTIFIER,
    @AddressId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, [Label], RecipientName, Phone, AddressLine, City, PostalCode, IsDefault
    FROM dbo.Addresses
    WHERE Id = @AddressId
      AND UserId = @UserId
      AND IsDeleted = 0;
END;

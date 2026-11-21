CREATE PROCEDURE dbo.Address_Update
    @UserId UNIQUEIDENTIFIER,
    @AddressId UNIQUEIDENTIFIER,
    @Label NVARCHAR(60),
    @RecipientName NVARCHAR(160),
    @Phone NVARCHAR(30),
    @AddressLine NVARCHAR(300),
    @City NVARCHAR(100),
    @PostalCode NVARCHAR(20),
    @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Addresses
    SET [Label] = @Label,
        RecipientName = @RecipientName,
        Phone = @Phone,
        AddressLine = @AddressLine,
        City = @City,
        PostalCode = @PostalCode,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @AddressId
      AND UserId = @UserId
      AND IsDeleted = 0;
END;

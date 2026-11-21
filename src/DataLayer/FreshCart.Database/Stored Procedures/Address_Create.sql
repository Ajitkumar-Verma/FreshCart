CREATE PROCEDURE dbo.Address_Create
    @AddressId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @Label NVARCHAR(60),
    @RecipientName NVARCHAR(160),
    @Phone NVARCHAR(30),
    @AddressLine NVARCHAR(300),
    @City NVARCHAR(100),
    @PostalCode NVARCHAR(20),
    @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Addresses
    (
        Id,
        UserId,
        [Label],
        RecipientName,
        Phone,
        AddressLine,
        City,
        PostalCode,
        IsDefault,
        CreatedBy
    )
    VALUES
    (
        @AddressId,
        @UserId,
        @Label,
        @RecipientName,
        @Phone,
        @AddressLine,
        @City,
        @PostalCode,
        0,
        @CreatedBy
    );
END;

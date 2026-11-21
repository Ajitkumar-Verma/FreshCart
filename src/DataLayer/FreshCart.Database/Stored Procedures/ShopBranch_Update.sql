CREATE PROCEDURE dbo.ShopBranch_Update
    @Id UNIQUEIDENTIFIER,
    @ShopId UNIQUEIDENTIFIER,
    @Name NVARCHAR(120),
    @AddressLine NVARCHAR(250),
    @City NVARCHAR(100),
    @PostalCode NVARCHAR(20),
    @Latitude DECIMAL(9,6) = NULL,
    @Longitude DECIMAL(9,6) = NULL,
    @IsActive BIT,
    @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.ShopBranches
    SET Name = @Name,
        AddressLine = @AddressLine,
        City = @City,
        PostalCode = @PostalCode,
        Latitude = @Latitude,
        Longitude = @Longitude,
        IsActive = @IsActive,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @Id
      AND ShopId = @ShopId
      AND IsDeleted = 0;

    SELECT Id, ShopId, Name, AddressLine, City, PostalCode, Latitude, Longitude, IsActive
    FROM dbo.ShopBranches
    WHERE Id = @Id
      AND ShopId = @ShopId
      AND IsDeleted = 0;
END;

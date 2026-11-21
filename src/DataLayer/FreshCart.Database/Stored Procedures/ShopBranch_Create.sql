CREATE PROCEDURE dbo.ShopBranch_Create
    @Id UNIQUEIDENTIFIER,
    @ShopId UNIQUEIDENTIFIER,
    @Name NVARCHAR(120),
    @AddressLine NVARCHAR(250),
    @City NVARCHAR(100),
    @PostalCode NVARCHAR(20),
    @Latitude DECIMAL(9,6) = NULL,
    @Longitude DECIMAL(9,6) = NULL,
    @IsActive BIT,
    @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.ShopBranches
    (
        Id,
        ShopId,
        Name,
        AddressLine,
        City,
        PostalCode,
        Latitude,
        Longitude,
        IsActive,
        CreatedBy
    )
    VALUES
    (
        @Id,
        @ShopId,
        @Name,
        @AddressLine,
        @City,
        @PostalCode,
        @Latitude,
        @Longitude,
        @IsActive,
        @CreatedBy
    );

    SELECT Id, ShopId, Name, AddressLine, City, PostalCode, Latitude, Longitude, IsActive
    FROM dbo.ShopBranches
    WHERE Id = @Id;
END;

CREATE PROCEDURE dbo.Shop_Create
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(160),
    @Slug NVARCHAR(180),
    @Description NVARCHAR(1000) = NULL,
    @OwnerUserId UNIQUEIDENTIFIER,
    @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Shops
    (
        Id,
        Name,
        Slug,
        Description,
        OwnerUserId,
        CreatedBy
    )
    VALUES
    (
        @Id,
        @Name,
        @Slug,
        @Description,
        @OwnerUserId,
        @CreatedBy
    );

    SELECT Id, Name, Slug, Description, OwnerUserId, Status, PlatformFeePercent
    FROM dbo.Shops
    WHERE Id = @Id;
END;

CREATE PROCEDURE dbo.Shop_Update
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(160),
    @Slug NVARCHAR(180),
    @Description NVARCHAR(1000) = NULL,
    @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Shops
    SET Name = @Name,
        Slug = @Slug,
        Description = @Description,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @Id
      AND IsDeleted = 0;

    SELECT Id, Name, Slug, Description, OwnerUserId, Status, PlatformFeePercent
    FROM dbo.Shops
    WHERE Id = @Id
      AND IsDeleted = 0;
END;

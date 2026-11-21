CREATE PROCEDURE dbo.Shop_GetBySlug
    @Slug NVARCHAR(180)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, Slug, Description, OwnerUserId, Status, PlatformFeePercent
    FROM dbo.Shops
    WHERE Slug = @Slug;
END;

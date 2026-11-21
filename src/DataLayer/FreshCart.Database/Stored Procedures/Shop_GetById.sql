CREATE PROCEDURE dbo.Shop_GetById
    @ShopId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, Slug, Description, OwnerUserId, Status, PlatformFeePercent
    FROM dbo.Shops
    WHERE Id = @ShopId
      AND IsDeleted = 0;
END;

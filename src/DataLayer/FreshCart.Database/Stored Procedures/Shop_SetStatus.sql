CREATE PROCEDURE dbo.Shop_SetStatus
    @ShopId UNIQUEIDENTIFIER,
    @Status TINYINT,
    @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Shops
    SET Status = @Status,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @ShopId
      AND IsDeleted = 0;

    SELECT Id, Name, Slug, Description, OwnerUserId, Status, PlatformFeePercent
    FROM dbo.Shops
    WHERE Id = @ShopId
      AND IsDeleted = 0;
END;

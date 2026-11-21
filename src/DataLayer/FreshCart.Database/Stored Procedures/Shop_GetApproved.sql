CREATE PROCEDURE dbo.Shop_GetApproved
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, Slug, Description, OwnerUserId, Status, PlatformFeePercent
    FROM dbo.Shops
    WHERE Status = 1
      AND IsDeleted = 0
    ORDER BY Name;
END;

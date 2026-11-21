CREATE PROCEDURE dbo.ShopMembership_Exists
    @ShopId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(CASE WHEN EXISTS
    (
        SELECT 1
        FROM dbo.ShopMemberships
        WHERE ShopId = @ShopId
          AND UserId = @UserId
          AND IsDeleted = 0
    ) THEN 1 ELSE 0 END AS BIT);
END;

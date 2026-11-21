CREATE PROCEDURE dbo.Address_SetDefault
    @UserId UNIQUEIDENTIFIER,
    @AddressId UNIQUEIDENTIFIER,
    @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Addresses
    SET IsDefault = CASE WHEN Id = @AddressId THEN 1 ELSE 0 END,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE UserId = @UserId
      AND IsDeleted = 0
      AND (Id = @AddressId OR IsDefault = 1);

    SELECT CAST(CASE WHEN EXISTS
    (
        SELECT 1
        FROM dbo.Addresses
        WHERE Id = @AddressId
          AND UserId = @UserId
          AND IsDeleted = 0
          AND IsDefault = 1
    ) THEN 1 ELSE 0 END AS BIT);
END;

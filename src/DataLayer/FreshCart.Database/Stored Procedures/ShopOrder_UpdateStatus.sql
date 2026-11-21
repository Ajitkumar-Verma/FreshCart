CREATE PROCEDURE dbo.ShopOrder_UpdateStatus
    @ShopOrderId UNIQUEIDENTIFIER,
    @Status TINYINT,
    @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.ShopOrders
    SET Status = @Status,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @ShopOrderId
      AND IsDeleted = 0;

    SELECT CAST(CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS BIT);
END;

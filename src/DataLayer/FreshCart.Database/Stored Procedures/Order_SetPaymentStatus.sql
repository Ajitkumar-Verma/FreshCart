CREATE PROCEDURE dbo.Order_SetPaymentStatus
    @OrderId UNIQUEIDENTIFIER,
    @Status TINYINT,
    @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Orders
    SET PaymentStatus = @Status,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @OrderId
      AND IsDeleted = 0;

    SELECT CAST(CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS BIT);
END;

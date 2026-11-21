CREATE PROCEDURE dbo.Payment_ConfirmCash
    @OrderId UNIQUEIDENTIFIER,
    @ReceiptReference NVARCHAR(200),
    @CashOnDeliveryMethod TINYINT,
    @PaidStatus TINYINT,
    @PendingStatus TINYINT,
    @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Payments
    SET Status = @PaidStatus,
        GatewayReference = @ReceiptReference,
        PaidAt = SYSUTCDATETIME(),
        UpdatedBy = @UpdatedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE OrderId = @OrderId
      AND Method = @CashOnDeliveryMethod
      AND Status = @PendingStatus
      AND IsDeleted = 0;

    SELECT Id, OrderId, Method, Status, Amount, GatewayReference, PaidAt
    FROM dbo.Payments
    WHERE OrderId = @OrderId
      AND Method = @CashOnDeliveryMethod
      AND Status = @PaidStatus
      AND IsDeleted = 0;
END;

CREATE PROCEDURE dbo.Refund_Settle
    @RefundId UNIQUEIDENTIFIER,
    @Status TINYINT,
    @PendingStatus TINYINT,
    @GatewayReference NVARCHAR(200) = NULL,
    @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Refunds
    SET Status = @Status,
        GatewayReference = @GatewayReference,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @RefundId
      AND Status = @PendingStatus
      AND IsDeleted = 0;

    SELECT refund.Id,
           refund.PaymentId,
           payment.OrderId,
           refund.Amount,
           refund.Reason,
           refund.Status,
           refund.GatewayReference,
           refund.CreatedOn
    FROM dbo.Refunds AS refund
    INNER JOIN dbo.Payments AS payment ON payment.Id = refund.PaymentId
    WHERE refund.Id = @RefundId
      AND refund.IsDeleted = 0;
END;

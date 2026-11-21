CREATE PROCEDURE dbo.Refund_GetById
    @RefundId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

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
      AND refund.IsDeleted = 0
      AND payment.IsDeleted = 0;
END;

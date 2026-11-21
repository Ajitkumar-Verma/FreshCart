CREATE PROCEDURE dbo.Refund_Create
    @Id UNIQUEIDENTIFIER,
    @PaymentId UNIQUEIDENTIFIER,
    @Amount DECIMAL(12,2),
    @Reason NVARCHAR(500),
    @Status TINYINT,
    @PendingStatus TINYINT,
    @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Refunds (Id, PaymentId, Amount, Reason, Status, CreatedBy)
    VALUES (@Id, @PaymentId, @Amount, @Reason, @PendingStatus, @CreatedBy);

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
    WHERE refund.Id = @Id;
END;

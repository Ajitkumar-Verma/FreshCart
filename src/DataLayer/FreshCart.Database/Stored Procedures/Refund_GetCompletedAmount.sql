CREATE PROCEDURE dbo.Refund_GetCompletedAmount
    @PaymentId UNIQUEIDENTIFIER,
    @CompletedStatus TINYINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COALESCE(SUM(Amount), 0)
    FROM dbo.Refunds
    WHERE PaymentId = @PaymentId
      AND Status = @CompletedStatus
      AND IsDeleted = 0;
END;

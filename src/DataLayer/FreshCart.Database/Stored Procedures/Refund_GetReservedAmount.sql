CREATE PROCEDURE dbo.Refund_GetReservedAmount
    @PaymentId UNIQUEIDENTIFIER,
    @RejectedStatus TINYINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COALESCE(SUM(Amount), 0)
    FROM dbo.Refunds
    WHERE PaymentId = @PaymentId
      AND Status <> @RejectedStatus
      AND IsDeleted = 0;
END;

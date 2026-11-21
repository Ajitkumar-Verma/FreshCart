CREATE PROCEDURE dbo.Payment_GetByOrderId
    @OrderId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, OrderId, Method, Status, Amount, GatewayReference, PaidAt
    FROM dbo.Payments
    WHERE OrderId = @OrderId
      AND IsDeleted = 0;
END;

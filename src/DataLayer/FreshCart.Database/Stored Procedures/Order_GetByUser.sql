CREATE PROCEDURE dbo.Order_GetByUser
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, OrderNumber, OrderStatus, PaymentStatus, Subtotal, DeliveryFee, DiscountAmount,
           GrandTotal, CurrencyCode, CreatedOn
    FROM dbo.Orders
    WHERE UserId = @UserId
      AND IsDeleted = 0
    ORDER BY CreatedOn DESC;
END;

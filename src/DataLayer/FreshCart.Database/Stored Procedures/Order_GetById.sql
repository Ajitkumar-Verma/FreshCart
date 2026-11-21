CREATE PROCEDURE dbo.Order_GetById
    @OrderId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, OrderNumber, OrderStatus, PaymentStatus, Subtotal, DeliveryFee, DiscountAmount,
           GrandTotal, CurrencyCode, CreatedOn
    FROM dbo.Orders
    WHERE Id = @OrderId
      AND (@UserId IS NULL OR UserId = @UserId)
      AND IsDeleted = 0;
END;

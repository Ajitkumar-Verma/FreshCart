CREATE PROCEDURE dbo.Review_CanReviewOrderItem
    @UserId UNIQUEIDENTIFIER,
    @ProductId UNIQUEIDENTIFIER,
    @OrderItemId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(CASE WHEN EXISTS
    (
        SELECT 1
        FROM dbo.OrderItems AS item
        INNER JOIN dbo.ShopOrders AS shopOrder ON shopOrder.Id = item.ShopOrderId
        INNER JOIN dbo.Orders AS customerOrder ON customerOrder.Id = shopOrder.OrderId
        INNER JOIN dbo.ProductVariants AS variant ON variant.Id = item.ProductVariantId
        WHERE item.Id = @OrderItemId
          AND item.IsDeleted = 0
          AND customerOrder.UserId = @UserId
          AND shopOrder.Status = 5
          AND variant.ProductId = @ProductId
    ) THEN 1 ELSE 0 END AS BIT);
END;

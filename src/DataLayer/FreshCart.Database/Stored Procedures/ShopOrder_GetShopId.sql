CREATE PROCEDURE dbo.ShopOrder_GetShopId
    @ShopOrderId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT shopOrder.Id,
           shopOrder.OrderId,
           shopOrder.ShopId,
           shopOrder.ShopBranchId,
           shopOrder.Status,
           shopOrder.DeliveryFee,
           customerOrder.OrderNumber,
           shopOrder.DeliveryFee + ISNULL(
               (
                   SELECT SUM(item.LineTotal)
                   FROM dbo.OrderItems AS item
                   WHERE item.ShopOrderId = shopOrder.Id
                     AND item.IsDeleted = 0
               ),
               0
           ) AS ShopOrderTotal,
           shopOrder.CreatedOn
    FROM dbo.ShopOrders AS shopOrder
    INNER JOIN dbo.Orders AS customerOrder ON customerOrder.Id = shopOrder.OrderId
    WHERE shopOrder.Id = @ShopOrderId
      AND shopOrder.IsDeleted = 0
      AND customerOrder.IsDeleted = 0;
END;

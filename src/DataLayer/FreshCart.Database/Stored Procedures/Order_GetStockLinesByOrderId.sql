CREATE PROCEDURE dbo.Order_GetStockLinesByOrderId
    @OrderId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT shopOrder.ShopBranchId,
           item.ProductVariantId,
           item.Quantity
    FROM dbo.ShopOrders AS shopOrder
    INNER JOIN dbo.OrderItems AS item ON item.ShopOrderId = shopOrder.Id
    WHERE shopOrder.OrderId = @OrderId
      AND shopOrder.IsDeleted = 0
      AND item.ProductVariantId IS NOT NULL
      AND item.IsDeleted = 0;
END;

CREATE PROCEDURE dbo.Order_GetStockLinesByShopOrderId
    @ShopOrderId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT shopOrder.ShopBranchId,
           item.ProductVariantId,
           item.Quantity
    FROM dbo.ShopOrders AS shopOrder
    INNER JOIN dbo.OrderItems AS item ON item.ShopOrderId = shopOrder.Id
    WHERE shopOrder.Id = @ShopOrderId
      AND shopOrder.IsDeleted = 0
      AND item.ProductVariantId IS NOT NULL
      AND item.IsDeleted = 0;
END;

CREATE PROCEDURE dbo.ShopOrder_Create
    @Id UNIQUEIDENTIFIER,
    @OrderId UNIQUEIDENTIFIER,
    @ShopId UNIQUEIDENTIFIER,
    @ShopBranchId UNIQUEIDENTIFIER,
    @DeliverySlotId UNIQUEIDENTIFIER,
    @DeliveryFee DECIMAL(10,2),
    @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.ShopOrders
    (
        Id, OrderId, ShopId, ShopBranchId, DeliverySlotId, DeliveryFee, CreatedBy
    )
    VALUES
    (
        @Id, @OrderId, @ShopId, @ShopBranchId, @DeliverySlotId, @DeliveryFee, @CreatedBy
    );

    SELECT Id
    FROM dbo.ShopOrders
    WHERE Id = @Id;
END;

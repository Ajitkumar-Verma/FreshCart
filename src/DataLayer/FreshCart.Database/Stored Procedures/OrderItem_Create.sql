CREATE PROCEDURE dbo.OrderItem_Create
    @Id UNIQUEIDENTIFIER,
    @ShopOrderId UNIQUEIDENTIFIER,
    @ProductVariantId UNIQUEIDENTIFIER,
    @ProductName NVARCHAR(180),
    @UnitName NVARCHAR(40),
    @Quantity DECIMAL(10,3),
    @UnitPrice DECIMAL(10,2),
    @LineTotal DECIMAL(12,2),
    @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.OrderItems
    (
        Id, ShopOrderId, ProductVariantId, ProductName, UnitName,
        Quantity, UnitPrice, LineTotal, CreatedBy
    )
    VALUES
    (
        @Id, @ShopOrderId, @ProductVariantId, @ProductName, @UnitName,
        @Quantity, @UnitPrice, @LineTotal, @CreatedBy
    );
END;

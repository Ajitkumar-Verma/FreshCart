CREATE PROCEDURE dbo.OrderStatusHistory_Create
    @Id UNIQUEIDENTIFIER,
    @OrderId UNIQUEIDENTIFIER,
    @ShopOrderId UNIQUEIDENTIFIER = NULL,
    @Status TINYINT,
    @Note NVARCHAR(500) = NULL,
    @ChangedByUserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.OrderStatusHistory (Id, OrderId, ShopOrderId, Status, Note, ChangedByUserId, CreatedBy)
    VALUES (@Id, @OrderId, @ShopOrderId, @Status, @Note, @ChangedByUserId, @ChangedByUserId);
END;

CREATE PROCEDURE dbo.StockMovement_Create
    @InventoryId UNIQUEIDENTIFIER,
    @QuantityChange DECIMAL(12,3),
    @Reason NVARCHAR(40),
    @Notes NVARCHAR(500),
    @ChangedByUserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT dbo.StockMovements
    (
        InventoryId,
        QuantityChange,
        Reason,
        Notes,
        ChangedByUserId
    )
    VALUES
    (
        @InventoryId,
        @QuantityChange,
        @Reason,
        @Notes,
        @ChangedByUserId
    );
END;

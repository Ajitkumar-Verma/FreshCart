CREATE PROCEDURE dbo.StockMovement_Create
    @Id UNIQUEIDENTIFIER,
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
        Id,
        InventoryId,
        QuantityChange,
        Reason,
        Notes,
        ChangedByUserId
    )
    VALUES
    (
        @Id,
        @InventoryId,
        @QuantityChange,
        @Reason,
        @Notes,
        @ChangedByUserId
    );
END;

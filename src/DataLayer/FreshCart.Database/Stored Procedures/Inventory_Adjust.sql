CREATE PROCEDURE dbo.Inventory_Adjust
    @InventoryId UNIQUEIDENTIFIER,
    @QuantityChange DECIMAL(12,3),
    @Reason NVARCHAR(40),
    @Notes NVARCHAR(500),
    @ChangedByUserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @StartedTransaction BIT = 0;

    IF @@TRANCOUNT = 0
    BEGIN
        BEGIN TRANSACTION;
        SET @StartedTransaction = 1;
    END
    ELSE
    BEGIN
        SAVE TRANSACTION InventoryAdjustSavepoint;
    END;

    BEGIN TRY
        UPDATE dbo.Inventory WITH (UPDLOCK, ROWLOCK)
        SET QuantityOnHand = QuantityOnHand + @QuantityChange,
            UpdatedOn = SYSUTCDATETIME(),
            UpdatedBy = @ChangedByUserId
        WHERE Id = @InventoryId
          AND IsDeleted = 0
          AND QuantityOnHand + @QuantityChange >= 0;

        IF @@ROWCOUNT <> 1
            THROW 50001, 'Inventory record not found or adjustment would make stock negative.', 1;

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

        IF @StartedTransaction = 1
            COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @StartedTransaction = 1 AND XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        ELSE IF @StartedTransaction = 0 AND XACT_STATE() = 1
            ROLLBACK TRANSACTION InventoryAdjustSavepoint;

        ;THROW;
    END CATCH;
END;

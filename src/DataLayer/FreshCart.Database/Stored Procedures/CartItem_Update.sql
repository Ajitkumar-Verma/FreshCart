CREATE PROCEDURE dbo.CartItem_Update
    @ItemId UNIQUEIDENTIFIER,
    @Quantity DECIMAL(10,3),
    @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.CartItems
    SET Quantity = @Quantity,
        IsDeleted = 0,
        DeletedBy = NULL,
        DeletedAt = NULL,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @ItemId;
END;

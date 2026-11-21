CREATE PROCEDURE dbo.CartItem_Create
    @ItemId UNIQUEIDENTIFIER,
    @CartId UNIQUEIDENTIFIER,
    @ProductVariantId UNIQUEIDENTIFIER,
    @Quantity DECIMAL(10,3),
    @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.CartItems
    (
        Id,
        CartId,
        ProductVariantId,
        Quantity,
        CreatedBy
    )
    VALUES
    (
        @ItemId,
        @CartId,
        @ProductVariantId,
        @Quantity,
        @CreatedBy
    );
END;

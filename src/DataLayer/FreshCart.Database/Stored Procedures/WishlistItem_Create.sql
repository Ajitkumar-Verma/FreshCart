CREATE PROCEDURE dbo.WishlistItem_Create
    @ItemId UNIQUEIDENTIFIER,
    @WishlistId UNIQUEIDENTIFIER,
    @ProductId UNIQUEIDENTIFIER,
    @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.WishlistItems
    (
        Id,
        WishlistId,
        ProductId,
        CreatedBy
    )
    VALUES
    (
        @ItemId,
        @WishlistId,
        @ProductId,
        @CreatedBy
    );
END;

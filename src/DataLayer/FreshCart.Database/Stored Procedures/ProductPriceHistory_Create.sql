CREATE PROCEDURE dbo.ProductPriceHistory_Create
    @Id UNIQUEIDENTIFIER,
    @ProductVariantId UNIQUEIDENTIFIER,
    @OldPrice DECIMAL(10,2) = NULL,
    @NewPrice DECIMAL(10,2),
    @ChangedByUserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.ProductPriceHistory
    (
        Id,
        ProductVariantId,
        OldPrice,
        NewPrice,
        ChangedByUserId,
        CreatedBy
    )
    VALUES
    (
        @Id,
        @ProductVariantId,
        @OldPrice,
        @NewPrice,
        @ChangedByUserId,
        @ChangedByUserId
    );
END;

CREATE PROCEDURE dbo.CartItem_GetByVariant
    @CartId UNIQUEIDENTIFIER,
    @ProductVariantId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ProductVariantId, Quantity, IsDeleted
    FROM dbo.CartItems
    WHERE CartId = @CartId
      AND ProductVariantId = @ProductVariantId;
END;

CREATE PROCEDURE dbo.ProductVariant_GetProductId
    @VariantId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ProductId
    FROM dbo.ProductVariants
    WHERE Id = @VariantId
      AND IsDeleted = 0;
END;

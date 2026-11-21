CREATE PROCEDURE dbo.ProductVariant_ClearDefault
    @ProductId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.ProductVariants
    SET IsDefault = 0
    WHERE ProductId = @ProductId
      AND IsDefault = 1
      AND IsDeleted = 0;
END;

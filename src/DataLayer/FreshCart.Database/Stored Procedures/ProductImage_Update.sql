CREATE PROCEDURE dbo.ProductImage_Update
    @Id UNIQUEIDENTIFIER, @ProductId UNIQUEIDENTIFIER, @ImageUrl NVARCHAR(2048),
    @AltText NVARCHAR(250) = NULL, @SortOrder INT, @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.ProductImages SET ImageUrl = @ImageUrl, AltText = @AltText, SortOrder = @SortOrder,
        UpdatedBy = @UpdatedBy, UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @Id AND ProductId = @ProductId AND IsDeleted = 0;
    SELECT Id, ProductId, ImageUrl, AltText, SortOrder
    FROM dbo.ProductImages WHERE Id = @Id AND ProductId = @ProductId AND IsDeleted = 0;
END;

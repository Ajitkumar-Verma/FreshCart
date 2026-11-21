CREATE PROCEDURE dbo.ProductImage_Create
    @Id UNIQUEIDENTIFIER, @ProductId UNIQUEIDENTIFIER, @ImageUrl NVARCHAR(2048),
    @AltText NVARCHAR(250) = NULL, @SortOrder INT, @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, AltText, SortOrder, CreatedBy)
    VALUES (@Id, @ProductId, @ImageUrl, @AltText, @SortOrder, @CreatedBy);
    SELECT Id, ProductId, ImageUrl, AltText, SortOrder FROM dbo.ProductImages WHERE Id = @Id;
END;

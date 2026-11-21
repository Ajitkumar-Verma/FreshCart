CREATE PROCEDURE dbo.Product_Create
    @ProductId UNIQUEIDENTIFIER,
    @ShopId UNIQUEIDENTIFIER,
    @CategoryId UNIQUEIDENTIFIER,
    @Name NVARCHAR(180),
    @Description NVARCHAR(2000) = NULL,
    @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Products
    (
        Id,
        ShopId,
        CategoryId,
        Name,
        Description,
        CreatedBy
    )
    VALUES
    (
        @ProductId,
        @ShopId,
        @CategoryId,
        @Name,
        @Description,
        @CreatedBy
    );
END;

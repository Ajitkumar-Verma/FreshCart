CREATE PROCEDURE dbo.Product_Update
    @ProductId UNIQUEIDENTIFIER,
    @CategoryId UNIQUEIDENTIFIER,
    @Name NVARCHAR(180),
    @Description NVARCHAR(2000) = NULL,
    @IsActive BIT,
    @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Products
    SET CategoryId = @CategoryId,
        Name = @Name,
        Description = @Description,
        IsActive = @IsActive,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @ProductId
      AND IsDeleted = 0;
END;

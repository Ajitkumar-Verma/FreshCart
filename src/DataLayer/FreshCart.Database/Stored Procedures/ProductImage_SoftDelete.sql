CREATE PROCEDURE dbo.ProductImage_SoftDelete
    @ProductId UNIQUEIDENTIFIER, @ImageId UNIQUEIDENTIFIER, @DeletedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.ProductImages SET IsDeleted = 1, DeletedBy = @DeletedBy, DeletedAt = SYSUTCDATETIME(),
        UpdatedBy = @DeletedBy, UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @ImageId AND ProductId = @ProductId AND IsDeleted = 0;
    SELECT CAST(CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS BIT);
END;

CREATE PROCEDURE dbo.Review_SetPublished
    @ReviewId UNIQUEIDENTIFIER,
    @IsPublished BIT,
    @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Reviews
    SET IsPublished = @IsPublished,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @ReviewId
      AND IsDeleted = 0;

    SELECT Id, ProductId, UserId, OrderItemId, Rating, Comment, IsPublished, CreatedOn
    FROM dbo.Reviews
    WHERE Id = @ReviewId
      AND IsDeleted = 0;
END;

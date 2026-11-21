CREATE PROCEDURE dbo.Review_Update
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @Rating TINYINT,
    @Comment NVARCHAR(1000) = NULL,
    @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Reviews
    SET Rating = @Rating,
        Comment = @Comment,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @Id
      AND UserId = @UserId
      AND IsDeleted = 0;

    SELECT Id, ProductId, UserId, OrderItemId, Rating, Comment, IsPublished, CreatedOn
    FROM dbo.Reviews
    WHERE Id = @Id
      AND UserId = @UserId
      AND IsDeleted = 0;
END;

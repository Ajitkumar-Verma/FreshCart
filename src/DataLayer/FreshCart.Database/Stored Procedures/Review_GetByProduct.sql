CREATE PROCEDURE dbo.Review_GetByProduct
    @ProductId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ProductId, UserId, OrderItemId, Rating, Comment, IsPublished, CreatedOn
    FROM dbo.Reviews
    WHERE ProductId = @ProductId
      AND IsPublished = 1
      AND IsDeleted = 0
    ORDER BY CreatedOn DESC;
END;

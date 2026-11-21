CREATE PROCEDURE dbo.Review_GetByOrderItem
    @UserId UNIQUEIDENTIFIER,
    @OrderItemId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ProductId, UserId, OrderItemId, Rating, Comment, IsPublished, CreatedOn
    FROM dbo.Reviews
    WHERE UserId = @UserId
      AND OrderItemId = @OrderItemId
      AND IsDeleted = 0;
END;

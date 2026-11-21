CREATE PROCEDURE dbo.Review_Create
    @Id UNIQUEIDENTIFIER,
    @ProductId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @OrderItemId UNIQUEIDENTIFIER,
    @Rating TINYINT,
    @Comment NVARCHAR(1000) = NULL,
    @IsPublished BIT,
    @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Reviews (Id, ProductId, UserId, OrderItemId, Rating, Comment, IsPublished, CreatedBy)
    VALUES (@Id, @ProductId, @UserId, @OrderItemId, @Rating, @Comment, @IsPublished, @CreatedBy);

    SELECT Id, ProductId, UserId, OrderItemId, Rating, Comment, IsPublished, CreatedOn
    FROM dbo.Reviews
    WHERE Id = @Id;
END;

CREATE PROCEDURE dbo.Wishlist_Create
    @WishlistId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @Name NVARCHAR(100),
    @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Wishlists
    (
        Id,
        UserId,
        Name,
        CreatedBy
    )
    VALUES
    (
        @WishlistId,
        @UserId,
        @Name,
        @CreatedBy
    );
END;

CREATE PROCEDURE dbo.Cart_Create
    @CartId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Carts
    (
        Id,
        UserId,
        CreatedBy
    )
    VALUES
    (
        @CartId,
        @UserId,
        @CreatedBy
    );
END;

CREATE PROCEDURE dbo.Cart_GetByUser
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, UserId
    FROM dbo.Carts
    WHERE UserId = @UserId
      AND IsDeleted = 0;
END;

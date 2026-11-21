CREATE PROCEDURE dbo.Category_Exists
    @CategoryId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST(CASE WHEN EXISTS
    (
        SELECT 1
        FROM dbo.Categories
        WHERE Id = @CategoryId
          AND IsDeleted = 0
          AND IsActive = 1
    ) THEN 1 ELSE 0 END AS BIT);
END;

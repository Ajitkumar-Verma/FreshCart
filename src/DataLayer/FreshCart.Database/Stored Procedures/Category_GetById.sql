CREATE PROCEDURE dbo.Category_GetById
    @CategoryId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ParentCategoryId, Name, Slug, IsActive, SortOrder
    FROM dbo.Categories
    WHERE Id = @CategoryId
      AND IsDeleted = 0
      AND IsActive = 1;
END;

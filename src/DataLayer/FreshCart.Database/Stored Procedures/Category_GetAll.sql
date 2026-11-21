CREATE PROCEDURE dbo.Category_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ParentCategoryId, Name, Slug, IsActive, SortOrder
    FROM dbo.Categories
    WHERE IsDeleted = 0
      AND IsActive = 1
    ORDER BY SortOrder, Name;
END;

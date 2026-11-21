CREATE PROCEDURE dbo.Category_GetBySlug
    @Slug NVARCHAR(120)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ParentCategoryId, Name, Slug, IsActive, SortOrder
    FROM dbo.Categories
    WHERE Slug = @Slug;
END;

CREATE PROCEDURE dbo.Category_Update
    @Id UNIQUEIDENTIFIER,
    @ParentCategoryId UNIQUEIDENTIFIER = NULL,
    @Name NVARCHAR(100),
    @Slug NVARCHAR(120),
    @IsActive BIT,
    @SortOrder INT,
    @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Categories
    SET ParentCategoryId = @ParentCategoryId,
        Name = @Name,
        Slug = @Slug,
        IsActive = @IsActive,
        SortOrder = @SortOrder,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @Id
      AND IsDeleted = 0;

    SELECT Id, ParentCategoryId, Name, Slug, IsActive, SortOrder
    FROM dbo.Categories
    WHERE Id = @Id
      AND IsDeleted = 0;
END;

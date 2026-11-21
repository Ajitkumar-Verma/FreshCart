CREATE PROCEDURE dbo.Category_Create
    @Id UNIQUEIDENTIFIER,
    @ParentCategoryId UNIQUEIDENTIFIER = NULL,
    @Name NVARCHAR(100),
    @Slug NVARCHAR(120),
    @IsActive BIT,
    @SortOrder INT,
    @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Categories
    (
        Id,
        ParentCategoryId,
        Name,
        Slug,
        IsActive,
        SortOrder,
        CreatedBy
    )
    VALUES
    (
        @Id,
        @ParentCategoryId,
        @Name,
        @Slug,
        @IsActive,
        @SortOrder,
        @CreatedBy
    );

    SELECT Id, ParentCategoryId, Name, Slug, IsActive, SortOrder
    FROM dbo.Categories
    WHERE Id = @Id;
END;

CREATE PROCEDURE dbo.Review_SoftDelete
    @ReviewId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Reviews
    SET IsDeleted = 1,
        IsPublished = 0,
        DeletedBy = @UserId,
        DeletedAt = SYSUTCDATETIME(),
        UpdatedBy = @UserId,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @ReviewId
      AND UserId = @UserId
      AND IsDeleted = 0;

    SELECT CAST(CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS BIT);
END;

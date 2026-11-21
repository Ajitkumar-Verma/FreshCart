CREATE PROCEDURE dbo.ShopBranch_SoftDelete
    @ShopId UNIQUEIDENTIFIER,
    @BranchId UNIQUEIDENTIFIER,
    @DeletedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.ShopBranches
    SET IsDeleted = 1,
        DeletedBy = @DeletedBy,
        DeletedAt = SYSUTCDATETIME(),
        UpdatedBy = @DeletedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @BranchId
      AND ShopId = @ShopId
      AND IsDeleted = 0;

    SELECT CAST(CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS BIT);
END;

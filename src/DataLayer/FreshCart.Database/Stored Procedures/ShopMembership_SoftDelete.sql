CREATE PROCEDURE dbo.ShopMembership_SoftDelete
    @ShopId UNIQUEIDENTIFIER,
    @MembershipId UNIQUEIDENTIFIER,
    @DeletedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.ShopMemberships
    SET IsActive = 0,
        IsDeleted = 1,
        DeletedBy = @DeletedBy,
        DeletedAt = SYSUTCDATETIME(),
        UpdatedBy = @DeletedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @MembershipId
      AND ShopId = @ShopId
      AND IsDeleted = 0;

    SELECT CAST(CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS BIT);
END;

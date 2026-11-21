CREATE PROCEDURE dbo.ShopMembership_Update
    @Id UNIQUEIDENTIFIER,
    @ShopId UNIQUEIDENTIFIER,
    @RoleName NVARCHAR(40),
    @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.ShopMemberships
    SET RoleName = @RoleName,
        IsActive = 1,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @Id
      AND ShopId = @ShopId
      AND IsDeleted = 0;

    SELECT Id, ShopId, UserId, RoleName, IsActive
    FROM dbo.ShopMemberships
    WHERE Id = @Id
      AND ShopId = @ShopId
      AND IsDeleted = 0;
END;

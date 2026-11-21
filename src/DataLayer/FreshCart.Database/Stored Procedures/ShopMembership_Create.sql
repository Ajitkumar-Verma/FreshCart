CREATE PROCEDURE dbo.ShopMembership_Create
    @Id UNIQUEIDENTIFIER,
    @ShopId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @RoleName NVARCHAR(40),
    @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.ShopMemberships (Id, ShopId, UserId, RoleName, IsActive, CreatedBy)
    VALUES (@Id, @ShopId, @UserId, @RoleName, 1, @CreatedBy);

    SELECT Id, ShopId, UserId, RoleName, IsActive
    FROM dbo.ShopMemberships
    WHERE Id = @Id;
END;

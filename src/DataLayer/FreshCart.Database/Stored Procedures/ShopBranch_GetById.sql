CREATE PROCEDURE dbo.ShopBranch_GetById
    @ShopId UNIQUEIDENTIFIER,
    @BranchId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, ShopId, Name, AddressLine, City, PostalCode, Latitude, Longitude, IsActive
    FROM dbo.ShopBranches
    WHERE Id = @BranchId
      AND ShopId = @ShopId
      AND IsDeleted = 0;
END;

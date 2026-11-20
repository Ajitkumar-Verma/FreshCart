namespace FreshCart.Repository;

/// <summary>Canonical names for SQL Server stored procedures called by repositories.</summary>
public static class StoredProcedure
{
    public static class Catalog
    {
        public const string ProductGetAvailable = "dbo.Product_GetAvailable";
        public const string ProductGetById = "dbo.Product_GetById";
    }

    public static class Auth
    {
        public const string UserGetByEmail = "dbo.Auth_UserGetByEmail";
        public const string UserGetRoles = "dbo.Auth_UserGetRoles";
        public const string RoleGetByNormalizedName = "dbo.Auth_RoleGetByNormalizedName";
        public const string UserHasActiveRole = "dbo.Auth_UserHasActiveRole";
        public const string UserCreate = "dbo.Auth_UserCreate";
        public const string UserRoleCreate = "dbo.Auth_UserRoleCreate";
    }

    public static class Inventory
    {
        public const string UpdateQuantity = "dbo.Inventory_UpdateQuantity";
        public const string CreateStockMovement = "dbo.StockMovement_Create";
    }
}


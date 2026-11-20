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
        public const string UserCreate = "dbo.Auth_UserCreate";
        public const string BootstrapPlatformAdmin = "dbo.Auth_UserBootstrapPlatformAdmin";
    }

    public static class Inventory
    {
        public const string Adjust = "dbo.Inventory_Adjust";
    }
}


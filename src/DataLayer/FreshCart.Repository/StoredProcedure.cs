namespace FreshCart.Repository;

/// <summary>Canonical names for SQL Server stored procedures called by repositories.</summary>
public static class StoredProcedure
{
    public static class Catalog
    {
        public const string CategoryGetAll = "dbo.Category_GetAll";
        public const string CategoryGetById = "dbo.Category_GetById";
        public const string CategoryGetBySlug = "dbo.Category_GetBySlug";
        public const string CategoryExists = "dbo.Category_Exists";
        public const string CategoryCreate = "dbo.Category_Create";
        public const string CategoryUpdate = "dbo.Category_Update";
        public const string CategorySoftDelete = "dbo.Category_SoftDelete";
        public const string ProductGetAvailable = "dbo.Product_GetAvailable";
        public const string ProductGetById = "dbo.Product_GetById";
        public const string ProductGetForManagement = "dbo.Product_GetForManagement";
        public const string ProductGetShopId = "dbo.Product_GetShopId";
        public const string ProductCreate = "dbo.Product_Create";
        public const string ProductVariantCreate = "dbo.ProductVariant_Create";
        public const string ProductPriceHistoryCreate = "dbo.ProductPriceHistory_Create";
        public const string ProductUpdate = "dbo.Product_Update";
        public const string ProductSoftDelete = "dbo.Product_SoftDelete";
        public const string ProductVariantGetAll = "dbo.ProductVariant_GetAll";
        public const string ProductVariantGetForManagement = "dbo.ProductVariant_GetForManagement";
        public const string ProductVariantGetById = "dbo.ProductVariant_GetById";
        public const string ProductVariantGetProductId = "dbo.ProductVariant_GetProductId";
        public const string ProductVariantClearDefault = "dbo.ProductVariant_ClearDefault";
        public const string ProductVariantUpdate = "dbo.ProductVariant_Update";
        public const string ProductVariantSoftDelete = "dbo.ProductVariant_SoftDelete";
        public const string ProductImageGetByProduct = "dbo.ProductImage_GetByProduct";
        public const string ProductImageGetById = "dbo.ProductImage_GetById";
        public const string ProductImageCreate = "dbo.ProductImage_Create";
        public const string ProductImageUpdate = "dbo.ProductImage_Update";
        public const string ProductImageSoftDelete = "dbo.ProductImage_SoftDelete";
    }

    public static class Auth
    {
        public const string UserGetByEmail = "dbo.Auth_UserGetByEmail";
        public const string UserGetRoles = "dbo.Auth_UserGetRoles";
        public const string RoleGetByNormalizedName = "dbo.Auth_RoleGetByNormalizedName";
        public const string UserHasActiveRole = "dbo.Auth_UserHasActiveRole";
        public const string UserCreate = "dbo.Auth_UserCreate";
        public const string UserRoleCreate = "dbo.Auth_UserRoleCreate";
        public const string UserGetAll = "dbo.Auth_UserGetAll";
        public const string UserGetById = "dbo.Auth_UserGetById";
        public const string UserGetRoleAssignments = "dbo.Auth_UserGetRoleAssignments";
        public const string UserGetActiveRoleUserCount = "dbo.Auth_UserGetActiveRoleUserCount";
        public const string UserUpdate = "dbo.Auth_UserUpdate";
        public const string UserSoftDelete = "dbo.Auth_UserSoftDelete";
    }

    public static class Inventory
    {
        public const string UpdateQuantity = "dbo.Inventory_UpdateQuantity";
        public const string CreateStockMovement = "dbo.StockMovement_Create";
        public const string GetByShop = "dbo.Inventory_GetByShop";
        public const string GetById = "dbo.Inventory_GetById";
        public const string GetByBranchAndVariant = "dbo.Inventory_GetByBranchAndVariant";
        public const string Create = "dbo.Inventory_Create";
    }

    public static class Orders
    {
        public const string GetByUser = "dbo.Order_GetByUser";
        public const string GetById = "dbo.Order_GetById";
        public const string GetShopOrders = "dbo.Order_GetShopOrders";
        public const string GetShopOrdersByOrderId = "dbo.Order_GetShopOrdersByOrderId";
        public const string GetStockLinesByOrderId = "dbo.Order_GetStockLinesByOrderId";
        public const string GetStockLinesByShopOrderId = "dbo.Order_GetStockLinesByShopOrderId";
        public const string Create = "dbo.Order_Create";
        public const string CreateShopOrder = "dbo.ShopOrder_Create";
        public const string CreateItem = "dbo.OrderItem_Create";
        public const string CreatePayment = "dbo.Payment_Create";
        public const string GetPaymentByOrderId = "dbo.Payment_GetByOrderId";
        public const string ConfirmCashPayment = "dbo.Payment_ConfirmCash";
        public const string CreateCouponUsage = "dbo.CouponUsage_Create";
        public const string GetShopId = "dbo.ShopOrder_GetShopId";
        public const string UpdateShopStatus = "dbo.ShopOrder_UpdateStatus";
        public const string SetOrderStatus = "dbo.Order_SetStatus";
        public const string SetPaymentStatus = "dbo.Payment_SetStatus";
        public const string SetOrderPaymentStatus = "dbo.Order_SetPaymentStatus";
        public const string AddStatusHistory = "dbo.OrderStatusHistory_Create";
    }

    public static class Refunds
    {
        public const string GetById = "dbo.Refund_GetById";
        public const string GetPending = "dbo.Refund_GetPending";
        public const string GetReservedAmount = "dbo.Refund_GetReservedAmount";
        public const string GetCompletedAmount = "dbo.Refund_GetCompletedAmount";
        public const string Create = "dbo.Refund_Create";
        public const string Settle = "dbo.Refund_Settle";
    }

    public static class Reviews
    {
        public const string GetByProduct = "dbo.Review_GetByProduct";
        public const string GetByOrderItem = "dbo.Review_GetByOrderItem";
        public const string CanReviewOrderItem = "dbo.Review_CanReviewOrderItem";
        public const string Create = "dbo.Review_Create";
        public const string Update = "dbo.Review_Update";
        public const string SetPublished = "dbo.Review_SetPublished";
        public const string SoftDelete = "dbo.Review_SoftDelete";
    }

    public static class Notifications
    {
        public const string GetByUser = "dbo.Notification_GetByUser";
        public const string MarkRead = "dbo.Notification_MarkRead";
        public const string SoftDelete = "dbo.Notification_SoftDelete";
    }

    public static class Coupons
    {
        public const string GetAll = "dbo.Coupon_GetAll";
        public const string GetById = "dbo.Coupon_GetById";
        public const string GetByCode = "dbo.Coupon_GetByCode";
        public const string Create = "dbo.Coupon_Create";
        public const string Update = "dbo.Coupon_Update";
        public const string SoftDelete = "dbo.Coupon_SoftDelete";
    }

    public static class Marketplace
    {
        public const string ShopGetApproved = "dbo.Shop_GetApproved";
        public const string ShopGetManaged = "dbo.Shop_GetManaged";
        public const string ShopGetById = "dbo.Shop_GetById";
        public const string ShopGetBySlug = "dbo.Shop_GetBySlug";
        public const string ShopCanManage = "dbo.Shop_CanManage";
        public const string ShopCreate = "dbo.Shop_Create";
        public const string ShopUpdate = "dbo.Shop_Update";
        public const string ShopSetStatus = "dbo.Shop_SetStatus";
        public const string ShopSoftDelete = "dbo.Shop_SoftDelete";
        public const string ShopBranchGetPublic = "dbo.ShopBranch_GetPublic";
        public const string ShopBranchGetById = "dbo.ShopBranch_GetById";
        public const string ShopBranchCreate = "dbo.ShopBranch_Create";
        public const string ShopBranchUpdate = "dbo.ShopBranch_Update";
        public const string ShopBranchSoftDelete = "dbo.ShopBranch_SoftDelete";
        public const string ShopMembershipGetByShop = "dbo.ShopMembership_GetByShop";
        public const string ShopMembershipGetById = "dbo.ShopMembership_GetById";
        public const string ShopMembershipExists = "dbo.ShopMembership_Exists";
        public const string ShopMembershipCreate = "dbo.ShopMembership_Create";
        public const string ShopMembershipUpdate = "dbo.ShopMembership_Update";
        public const string ShopMembershipSoftDelete = "dbo.ShopMembership_SoftDelete";
        public const string DeliveryZoneGetByPostalCode = "dbo.DeliveryZone_GetByPostalCode";
        public const string DeliveryZoneGetByBranch = "dbo.DeliveryZone_GetByBranch";
        public const string DeliveryZoneGetById = "dbo.DeliveryZone_GetById";
        public const string DeliveryZoneCreate = "dbo.DeliveryZone_Create";
        public const string DeliveryZoneUpdate = "dbo.DeliveryZone_Update";
        public const string DeliveryZoneSoftDelete = "dbo.DeliveryZone_SoftDelete";
        public const string DeliverySlotGetByBranch = "dbo.DeliverySlot_GetByBranch";
        public const string DeliverySlotGetById = "dbo.DeliverySlot_GetById";
        public const string DeliverySlotCreate = "dbo.DeliverySlot_Create";
        public const string DeliverySlotUpdate = "dbo.DeliverySlot_Update";
        public const string DeliverySlotSoftDelete = "dbo.DeliverySlot_SoftDelete";
    }

    public static class Shopping
    {
        public const string AddressGetAll = "dbo.Address_GetAll";
        public const string AddressGetById = "dbo.Address_GetById";
        public const string AddressCreate = "dbo.Address_Create";
        public const string AddressUpdate = "dbo.Address_Update";
        public const string AddressSetDefault = "dbo.Address_SetDefault";
        public const string AddressSoftDelete = "dbo.Address_SoftDelete";
        public const string CartGetByUser = "dbo.Cart_GetByUser";
        public const string CartCreate = "dbo.Cart_Create";
        public const string CartItemGetAll = "dbo.CartItem_GetAll";
        public const string CartItemGetByVariant = "dbo.CartItem_GetByVariant";
        public const string CartItemCreate = "dbo.CartItem_Create";
        public const string CartItemUpdate = "dbo.CartItem_Update";
        public const string CartItemSoftDelete = "dbo.CartItem_SoftDelete";
        public const string WishlistGetAll = "dbo.Wishlist_GetAll";
        public const string WishlistGetById = "dbo.Wishlist_GetById";
        public const string WishlistGetByName = "dbo.Wishlist_GetByName";
        public const string WishlistCreate = "dbo.Wishlist_Create";
        public const string WishlistUpdate = "dbo.Wishlist_Update";
        public const string WishlistSoftDelete = "dbo.Wishlist_SoftDelete";
        public const string WishlistItemGetAll = "dbo.WishlistItem_GetAll";
        public const string WishlistItemGetId = "dbo.WishlistItem_GetId";
        public const string WishlistItemCreate = "dbo.WishlistItem_Create";
        public const string WishlistItemRestore = "dbo.WishlistItem_Restore";
        public const string WishlistItemSoftDelete = "dbo.WishlistItem_SoftDelete";
    }
}


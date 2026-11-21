using FreshCart.Common.Auth;
using FreshCart.IService.Auth;
using FreshCart.IService.Catalog;
using FreshCart.IService.Inventory;
using FreshCart.IService.Marketplace;
using FreshCart.IService.Orders;
using FreshCart.IService.Reviews;
using FreshCart.IService.Notifications;
using FreshCart.IService.Shopping;
using FreshCart.Service.Auth;
using FreshCart.Service.Catalog;
using FreshCart.Service.Inventory;
using FreshCart.Service.Marketplace;
using FreshCart.Service.Orders;
using FreshCart.Service.Reviews;
using FreshCart.Service.Notifications;
using FreshCart.Service.Shopping;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace FreshCart.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddFreshCartServices(this IServiceCollection services)
    {
        // Business services are scoped so they share request-scoped repositories and unit of work.
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductImageService, ProductImageService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IShopService, ShopService>();
        services.AddScoped<IShopBranchService, ShopBranchService>();
        services.AddScoped<IShopMembershipService, ShopMembershipService>();
        services.AddScoped<IDeliveryConfigurationService, DeliveryConfigurationService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IRefundService, RefundService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IWishlistService, WishlistService>();
        services.AddScoped<ICouponService, CouponService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher<UserAccount>, PasswordHasher<UserAccount>>();
        return services;
    }
}

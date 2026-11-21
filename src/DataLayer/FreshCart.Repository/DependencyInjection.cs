using FreshCart.Common.Configuration;
using FreshCart.Common.Constants;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Auth;
using FreshCart.IRepository.Catalog;
using FreshCart.IRepository.Inventory;
using FreshCart.IRepository.Marketplace;
using FreshCart.IRepository.Orders;
using FreshCart.IRepository.Reviews;
using FreshCart.IRepository.Notifications;
using FreshCart.IRepository.Shopping;
using FreshCart.Repository.Auth;
using FreshCart.Repository.Catalog;
using FreshCart.Repository.Inventory;
using FreshCart.Repository.Marketplace;
using FreshCart.Repository.Orders;
using FreshCart.Repository.Reviews;
using FreshCart.Repository.Notifications;
using FreshCart.Repository.Shopping;
using FreshCart.Repository.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace FreshCart.Repository;

public static class DependencyInjection
{
    public static IServiceCollection AddFreshCartRepositories(this IServiceCollection services, string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException(ConfigurationErrorMessages.DatabaseConnectionMissing, nameof(connectionString));
        
        services.AddSingleton(new DatabaseOptions { FreshCart = connectionString });
        services.AddSingleton<IDbConnectionFactory, SqlServerConnectionFactory>();
        services.AddSingleton<IIdGenerator, GuidIdGenerator>();
        
        // A request gets one unit of work/connection shared by all scoped repositories.
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductImageRepository, ProductImageRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IShopRepository, ShopRepository>();
        services.AddScoped<IShopBranchRepository, ShopBranchRepository>();
        services.AddScoped<IShopMembershipRepository, ShopMembershipRepository>();
        services.AddScoped<IDeliveryConfigurationRepository, DeliveryConfigurationRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IRefundRepository, RefundRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IWishlistRepository, WishlistRepository>();
        services.AddScoped<ICouponRepository, CouponRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        
        return services;
    }
}





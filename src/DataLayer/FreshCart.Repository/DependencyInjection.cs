using FreshCart.Common.Configuration;
using FreshCart.Common.Constants;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Catalog;
using FreshCart.IRepository.Auth;
using FreshCart.Repository.Auth;
using FreshCart.Repository.Catalog;
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
        services.AddScoped<IUserRepository, UserRepository>();
        
        return services;
    }
}





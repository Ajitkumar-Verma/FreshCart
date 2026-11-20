using FreshCart.Common.Configuration;
using FreshCart.Common.Persistence;
using FreshCart.IRepository.Catalog;
using FreshCart.Repository.Catalog;
using FreshCart.Repository.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace FreshCart.Repository;

public static class DependencyInjection
{
    public static IServiceCollection AddFreshCartRepositories(this IServiceCollection services, string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("The FreshCart database connection string is required.", nameof(connectionString));
        services.AddSingleton(new DatabaseOptions { FreshCart = connectionString });
        services.AddSingleton<IDbConnectionFactory, SqlServerConnectionFactory>();
        services.AddSingleton<IIdGenerator, GuidIdGenerator>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductRepository, ProductRepository>();
        return services;
    }
}



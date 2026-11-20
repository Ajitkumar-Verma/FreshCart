using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using FreshCart.Common.Catalog;
using FreshCart.IRepository.Catalog;

namespace FreshCart.Repository.Catalog;

/// <summary>SQL Server implementation of the catalog repository.</summary>
public sealed class ProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(string connectionString)
    {
        _connectionString = string.IsNullOrWhiteSpace(connectionString) ? throw new ArgumentException("A database connection string is required.", nameof(connectionString)) : connectionString;
    }

    public async Task<IReadOnlyList<Product>> GetAvailableAsync(int? shopId = null, int? categoryId = null, string? search = null, CancellationToken cancellationToken = default)
    {
        const string sql = "dbo.Product_GetAvailable";
        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@ShopId", shopId, DbType.Int32);
        AddParameter(command, "@CategoryId", categoryId, DbType.Int32);
        AddParameter(command, "@Search", string.IsNullOrWhiteSpace(search) ? null : search.Trim(), DbType.String);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var products = new List<Product>();
        while (await reader.ReadAsync(cancellationToken)) products.Add(ReadProduct(reader));
        return products;
    }

    public async Task<Product?> GetByIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        const string sql = "dbo.Product_GetById";
        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@ProductId", productId, DbType.Int32);
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow, cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? ReadProduct(reader) : null;
    }

    private DbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    private static void AddParameter(DbCommand command, string name, object? value, DbType type)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.DbType = type;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }

    private static Product ReadProduct(DbDataReader reader) => new(
        reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetString(3),
        reader.IsDBNull(4) ? null : reader.GetString(4), reader.GetDecimal(5), reader.GetString(6), reader.GetBoolean(7));
}

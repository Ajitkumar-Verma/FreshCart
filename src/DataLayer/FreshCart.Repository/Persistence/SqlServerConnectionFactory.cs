using FreshCart.Common.Configuration;
using FreshCart.Common.Persistence;
using Microsoft.Data.SqlClient;

namespace FreshCart.Repository.Persistence;

public sealed class SqlServerConnectionFactory(DatabaseOptions options) : IDbConnectionFactory
{
    public System.Data.Common.DbConnection CreateConnection() => new SqlConnection(options.FreshCart);
}

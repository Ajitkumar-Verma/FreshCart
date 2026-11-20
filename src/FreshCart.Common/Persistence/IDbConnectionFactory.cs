using System.Data.Common;

namespace FreshCart.Common.Persistence;

public interface IDbConnectionFactory
{
    DbConnection CreateConnection();
}

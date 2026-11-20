using System.Data;
using System.Data.Common;

namespace FreshCart.Common.Persistence;

public interface IUnitOfWork : IAsyncDisposable
{
    DbTransaction? Transaction { get; }

    Task<DbConnection> GetConnectionAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);
}

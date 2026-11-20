using System.Data;
using System.Data.Common;

namespace FreshCart.Common.Persistence;

public interface IUnitOfWork : IAsyncDisposable
{
    DbTransaction? Transaction { get; }

    /// <summary>Runs an operation with this unit of work's opened connection and current transaction.</summary>
    Task<TResult> WithConnectionAsync<TResult>(Func<DbConnection, DbTransaction?, Task<TResult>> operation, CancellationToken cancellationToken = default);

    /// <summary>Runs an operation with this unit of work's opened connection and current transaction.</summary>
    Task WithConnectionAsync(Func<DbConnection, DbTransaction?, Task> operation, CancellationToken cancellationToken = default);

    Task<DbConnection> GetConnectionAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);
}

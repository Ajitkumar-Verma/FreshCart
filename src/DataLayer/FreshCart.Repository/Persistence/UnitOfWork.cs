using System.Data;
using System.Data.Common;
using FreshCart.Common.Persistence;

namespace FreshCart.Repository.Persistence;

public sealed class UnitOfWork(IDbConnectionFactory connectionFactory) : IUnitOfWork
{
    private DbConnection? _connection;
    public DbTransaction? Transaction { get; private set; }

    public async Task<DbConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        _connection ??= connectionFactory.CreateConnection();
        if (_connection.State != ConnectionState.Open) await _connection.OpenAsync(cancellationToken);
        return _connection;
    }

    public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
    {
        if (Transaction is not null) throw new InvalidOperationException("A unit-of-work transaction is already active.");
        var connection = await GetConnectionAsync(cancellationToken);
        Transaction = await connection.BeginTransactionAsync(isolationLevel, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (Transaction is null) return 0;
        await CommitAsync(cancellationToken);
        return 1;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (Transaction is null) throw new InvalidOperationException("No active transaction to commit.");
        await Transaction.CommitAsync(cancellationToken);
        await Transaction.DisposeAsync();
        Transaction = null;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (Transaction is null) return;
        await Transaction.RollbackAsync(cancellationToken);
        await Transaction.DisposeAsync();
        Transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (Transaction is not null)
        {
            try { await Transaction.RollbackAsync(); }
            finally { await Transaction.DisposeAsync(); Transaction = null; }
        }
        if (_connection is not null) await _connection.DisposeAsync();
    }
}

namespace FreshCart.Common.Constants;

public static class UnitOfWorkErrorMessages
{
    public const string TransactionAlreadyActive = "A unit-of-work transaction is already active.";
    public const string NoTransactionToCommit = "No active transaction to commit.";
}

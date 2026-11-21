namespace FreshCart.Common.Auth;

public sealed class UserDeletionResponse
{
    public UserDeletionResponse(Guid userId, bool isDeleted)
    {
        UserId = userId;
        IsDeleted = isDeleted;
    }

    public Guid UserId { get; }

    public bool IsDeleted { get; }
}

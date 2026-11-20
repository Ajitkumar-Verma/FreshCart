namespace FreshCart.Common.Auth;

public sealed record CreateUserResult(Guid UserId, bool AlreadyExists);

namespace FreshCart.Common.Auth;

public sealed record UserDeletionResponse(Guid UserId, bool IsDeleted);

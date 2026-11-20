using FreshCart.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Auth;

public sealed record CreateSuperUserRequest(
    [Required, EmailAddress, MaxLength(256)] string Email,
    [Required, MinLength(AuthPolicy.MinimumPasswordLength), MaxLength(AuthPolicy.MaximumPasswordLength)] string Password,
    [Required, MaxLength(160)] string DisplayName);

using FreshCart.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Auth;

public sealed record LoginRequest(
    [Required, EmailAddress, MaxLength(256)] string Email,
    [Required, MaxLength(AuthPolicy.MaximumPasswordLength)] string Password);

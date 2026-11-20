using System.ComponentModel.DataAnnotations;

namespace FreshCart.Common.Auth;

public sealed record UpdateManagedUserRequest(
    [Required, EmailAddress, MaxLength(256)] string Email,
    [Required, MaxLength(160)] string DisplayName,
    [Phone, MaxLength(30)] string? PhoneNumber);

using FreshCart.Common.Exceptions;

namespace FreshCart.Common.Validation;

public static class Guard
{
    public static T NotNull<T>(T? value, string name) where T : class =>
        value ?? throw new ValidationAppException($"{name} is required.");

    public static string NotNullOrWhiteSpace(string? value, string name) =>
        string.IsNullOrWhiteSpace(value) ? throw new ValidationAppException($"{name} is required.") : value.Trim();

    public static int Positive(int value, string name) =>
        value > 0 ? value : throw new ValidationAppException($"{name} must be greater than zero.");

    public static decimal NonNegative(decimal value, string name) =>
        value >= 0 ? value : throw new ValidationAppException($"{name} cannot be negative.");

    public static Guid NotEmpty(Guid value, string name) =>
        value != Guid.Empty ? value : throw new ValidationAppException($"{name} must be a non-empty identifier.");

    public static void IsTrue(bool condition, string message)
    {
        if (!condition) throw new ValidationAppException(message);
    }

    public static void IsFalse(bool condition, string message)
    {
        if (condition) throw new ValidationAppException(message);
    }
}

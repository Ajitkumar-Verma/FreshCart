namespace FreshCart.Common.Constants;

public static class PaymentStatuses
{
    public const byte Pending = 0;

    public const byte Paid = 1;

    public const byte Cancelled = 2;

    public const byte Failed = 3;

    public const byte PartiallyRefunded = 4;

    public const byte Refunded = 5;
}

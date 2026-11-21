namespace FreshCart.Common.Constants;

public static class OrderStatuses
{
    public const byte New = 0;

    public const byte Confirmed = 1;

    public const byte Preparing = 2;

    public const byte Ready = 3;

    public const byte OutForDelivery = 4;

    public const byte Delivered = 5;

    public const byte Cancelled = 6;
}

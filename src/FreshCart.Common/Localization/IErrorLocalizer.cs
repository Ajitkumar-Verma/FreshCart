namespace FreshCart.Common.Localization;

public interface IErrorLocalizer
{
    string GetMessage(string errorCode, string? language = null);
}

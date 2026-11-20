namespace FreshCart.Common.Constants;

public static class ApiRoutes
{
    public const string Auth = "api/auth";
    public const string AuthRegister = "register";
    public const string AuthLogin = "login";
    public const string SuperUsers = "api/superusers";
    public const string BootstrapSuperUser = "bootstrap";
    public const string Products = "api/products";
    public const string ProductById = "{productId:guid}";
    public const string Users = "api/users";
    public const string UserById = "{userId:guid}";
}

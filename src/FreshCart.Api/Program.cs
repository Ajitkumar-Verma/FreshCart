using FreshCart.Common.Configuration;
using FreshCart.Common.Constants;
using FreshCart.Common.Localization;
using FreshCart.Repository;
using FreshCart.Service;
using FreshCart.Api.Middleware;
using FreshCart.Api.Constants;
using FreshCart.Api.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    var localConfiguration = new ConfigurationBuilder()
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false)
        .Build();

    foreach (var setting in localConfiguration.AsEnumerable())
    {
        if (setting.Value is not null && string.IsNullOrWhiteSpace(builder.Configuration[setting.Key]))
        {
            builder.Configuration[setting.Key] = setting.Value;
        }
    }
}

var jwtSettings = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException(ConfigurationErrorMessages.JwtSettingsMissing);
if (string.IsNullOrWhiteSpace(jwtSettings.Issuer) || string.IsNullOrWhiteSpace(jwtSettings.Audience) ||
    jwtSettings.ExpirationMinutes is < 1 or > 1440)
    throw new InvalidOperationException(ConfigurationErrorMessages.JwtSettingsInvalid);
if (Encoding.UTF8.GetByteCount(jwtSettings.SigningKey) < AuthPolicy.MinimumJwtSigningKeyBytes)
    throw new InvalidOperationException(ConfigurationErrorMessages.JwtSigningKeyMissing);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<BootstrapOptions>(builder.Configuration.GetSection(BootstrapOptions.SectionName));

builder.Services.AddFreshCartServices();

var connectionString = builder.Configuration.GetConnectionString("FreshCart")
    ?? throw new InvalidOperationException(ConfigurationErrorMessages.DatabaseConnectionMissing);

builder.Services.AddFreshCartRepositories(connectionString);
builder.Services.AddSingleton<IErrorLocalizer, ErrorLocalizer>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
    options.AddSecurityDefinition(SwaggerConstants.BearerSecurityScheme, new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = SwaggerConstants.AuthorizationHeader,
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = SwaggerConstants.HttpAuthorizationScheme,
        BearerFormat = SwaggerConstants.JwtTokenFormat,
        Description = "Enter a JWT access token. Swagger adds the Bearer prefix to requests."
    });
    options.OperationFilter<AuthorizeOperationFilter>();
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var localizer = context.HttpContext.RequestServices.GetRequiredService<IErrorLocalizer>();
        var language = context.HttpContext.Request.Headers[HttpHeaderNames.AcceptLanguage].FirstOrDefault();
        const int statusCode = StatusCodes.Status400BadRequest;
        var response = new FreshCart.Common.Contracts.ApiResponse<object?>(
            statusCode,
            false,
            AppErrorCodes.Validation,
            localizer.GetMessage(AppErrorCodes.Validation, language),
            null,
            context.HttpContext.TraceIdentifier,
            context.HttpContext.Request.Path);

        return new ObjectResult(response) { StatusCode = statusCode };
    };
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            RoleClaimType = TokenClaimNames.Role,
            NameClaimType = TokenClaimNames.Name
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                await WriteAuthorizationErrorAsync(context.HttpContext, StatusCodes.Status401Unauthorized, AppErrorCodes.Unauthorized);
            },
            OnForbidden = async context =>
            {
                await WriteAuthorizationErrorAsync(context.HttpContext, StatusCodes.Status403Forbidden, AppErrorCodes.Forbidden);
            }
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});

var app = builder.Build();
_ = app.Services.GetRequiredService<IErrorLocalizer>();
app.UseMiddleware<ApiExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

static async Task WriteAuthorizationErrorAsync(HttpContext context, int statusCode, string errorCode)
{
    if (context.Response.HasStarted) return;
    var localizer = context.RequestServices.GetRequiredService<IErrorLocalizer>();
    var language = context.Request.Headers[HttpHeaderNames.AcceptLanguage].FirstOrDefault();
    context.Response.StatusCode = statusCode;
    await context.Response.WriteAsJsonAsync(
        new FreshCart.Common.Contracts.ApiResponse<object?>(
            statusCode,
            false,
            errorCode,
            localizer.GetMessage(errorCode, language),
            null,
            context.TraceIdentifier,
            context.Request.Path),
        context.RequestAborted);
}

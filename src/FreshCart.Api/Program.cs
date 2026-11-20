using FreshCart.Common.Configuration;
using FreshCart.Common.Constants;
using FreshCart.Common.Localization;
using FreshCart.Repository;
using FreshCart.Service;
using FreshCart.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException(ConfigurationErrorMessages.JwtSettingsMissing);
if (string.IsNullOrWhiteSpace(jwtSettings.Issuer) || string.IsNullOrWhiteSpace(jwtSettings.Audience) ||
    Encoding.UTF8.GetByteCount(jwtSettings.SigningKey) < 32 || jwtSettings.ExpirationMinutes is < 1 or > 1440)
    throw new InvalidOperationException(ConfigurationErrorMessages.JwtSettingsInvalid);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<BootstrapOptions>(builder.Configuration.GetSection(BootstrapOptions.SectionName));
builder.Services.AddFreshCartServices();
var connectionString = builder.Configuration.GetConnectionString("FreshCart")
    ?? throw new InvalidOperationException(ConfigurationErrorMessages.DatabaseConnectionMissing);
builder.Services.AddFreshCartRepositories(connectionString);
builder.Services.AddSingleton<IErrorLocalizer, ErrorLocalizer>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
    });
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});

var app = builder.Build();
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





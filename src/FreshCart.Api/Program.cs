using FreshCart.Common.Localization;
using FreshCart.Repository;
using FreshCart.Service;
using FreshCart.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFreshCartServices();
var connectionString = builder.Configuration.GetConnectionString("FreshCart")
    ?? throw new InvalidOperationException("Connection string 'FreshCart' is missing.");
builder.Services.AddFreshCartRepositories(connectionString);
builder.Services.AddSingleton<IErrorLocalizer, ErrorLocalizer>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseMiddleware<ApiExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();
app.Run();

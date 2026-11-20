using FreshCart.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFreshCartServices();
builder.Services.AddScoped<FreshCart.IRepository.Catalog.IProductRepository>(services =>
    new FreshCart.Repository.Catalog.ProductRepository(
        services.GetRequiredService<IConfiguration>().GetConnectionString("FreshCart")
        ?? throw new InvalidOperationException("Connection string 'FreshCart' is missing.")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

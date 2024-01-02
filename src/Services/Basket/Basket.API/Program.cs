using Basket.API.Entities.Repositories;
using Basket.API.GrpcServices;
using Discount.Grpc.Protos;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddStackExchangeRedisCache(options => 
{ 
    options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
});

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(
    option => { option.Address = new Uri(builder.Configuration["GrpcSettings:DiscountGrpcUrl"]); });

builder.Services.AddScoped<DiscountGrpcService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1",new Microsoft.OpenApi.Models.OpenApiInfo() { Version="v1", Title="Basket.API" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API v1");
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();

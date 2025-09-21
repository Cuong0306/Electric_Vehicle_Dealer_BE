using ElectricVehicleDealer.BLL.Services;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ??ng ký Repository & Service
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
// N?u có repository riêng
builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<BrandRepository>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<PaymentRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

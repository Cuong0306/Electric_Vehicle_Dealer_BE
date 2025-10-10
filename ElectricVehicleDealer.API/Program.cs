using CloudinaryDotNet;
using ElectricVehicleDealer.BLL.Services;
using ElectricVehicleDealer.BLL.Services.Implementations;
using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Repositories;
using ElectricVehicleDealer.DAL.Repositories.Implementations;
using ElectricVehicleDealer.DAL.Repositories.Interfaces;
using ElectricVehicleDealer.DAL.UnitOfWork;
using ElectricVehicleDealer.DTO.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// --- Controllers (GỘP một lần) ---
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // enum -> string (Pending, Active, ...)
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        // converter DateTime của bạn
        o.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
        // Nếu có model tham chiếu vòng:
        // o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- DbContext ---
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- UnitOfWork/Repositories/Services ---
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services (mỗi service đăng ký 1 lần)
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IDealerService, DealerService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IAgreementsService, AgreementsService>();
builder.Services.AddScoped<IStoreService, StoreService>();

// Repositories (nếu dùng interface thì đăng ký qua interface)
builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<BrandRepository>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<PaymentRepository>();
builder.Services.AddScoped<PromotionRepository>();
builder.Services.AddScoped<IAgreementsRepository, AgreementsRepository>();
builder.Services.AddScoped<StoreRepository>();

// --- Cloudinary ---
var cloud = builder.Configuration.GetSection("Cloudinary");
builder.Services.AddSingleton(new Cloudinary(new Account(
    cloud["CloudName"], cloud["ApiKey"], cloud["ApiSecret"]
)));
// QUAN TRỌNG: đăng ký service interface để controller resolve được
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", p =>
        p.WithOrigins("http://localhost:5173")
         .AllowAnyHeader()
         .AllowAnyMethod());
});

// --- Auth/JWT ---
var key = builder.Configuration["Jwt:Key"]
          ?? throw new InvalidOperationException("Missing Jwt:Key in configuration");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();      // giúp thấy lỗi thật khi mở /swagger/v1/swagger.json
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ElectricVehicleDealer.API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

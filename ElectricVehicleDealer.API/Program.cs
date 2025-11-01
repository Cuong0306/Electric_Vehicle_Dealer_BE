using CloudinaryDotNet;
using ElectricVehicleDealer.API.Middlewares;
using ElectricVehicleDealer.BLL.Intergations.Interfaces;
using ElectricVehicleDealer.BLL.Intergations.Implementations;
using ElectricVehicleDealer.DTO.Config;
using ElectricVehicleDealer.BLL.Services;
using ElectricVehicleDealer.BLL.Services.Implementations;
using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.BLL.Services.Interfaces.Implementations;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Repositories.Implementations;
using ElectricVehicleDealer.DAL.Repositories.Interfaces;
using ElectricVehicleDealer.DAL.Services.Interfaces;
using ElectricVehicleDealer.DAL.UnitOfWork;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
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
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IDealerRepository, DealerRepository>();


//builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IAgreementsService, AgreementsService>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<ITestAppointmentService, TestAppointmentService>();
builder.Services.AddScoped<IQuoteService, QuoteService>();
builder.Services.AddScoped<IStaffService, StaffService>();

//builder.Services.AddScoped<IPaymentService,PaymentService>();

//builder.Services.AddHttpClient<PayOsService>();


// Repositories (nếu dùng interface thì đăng ký qua interface)
builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<BrandRepository>();
builder.Services.AddScoped<OrderRepository>();
//builder.Services.AddScoped<PaymentRepository>();
builder.Services.AddScoped<PromotionRepository>();
builder.Services.AddScoped<IAgreementsRepository, AgreementsRepository>();
builder.Services.AddScoped<StoreRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IStorageRepository, StorageRepository>();

// --- Cloudinary ---
var cloud = builder.Configuration.GetSection("Cloudinary");
builder.Services.AddSingleton(new Cloudinary(new Account(
    cloud["CloudName"], cloud["ApiKey"], cloud["ApiSecret"]
)));
// QUAN TRỌNG: đăng ký service interface để controller resolve được
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();


// --- PayOS ---
// Đọc cấu hình từ appsettings.json
builder.Services.Configure<PayOsSettings>(
    builder.Configuration.GetSection("PayOS")
);

builder.Services.AddHttpClient<IPayOsService, PayOsService>();
// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", p =>
        p.WithOrigins("http://localhost:5173")
         .AllowAnyHeader()
         .AllowAnyMethod());
});
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// --- Auth/JWT ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ElectricVehicleDealer API",
        Version = "v1"
    });

    // Đường dẫn XML doc an toàn theo assembly hiện tại
    var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        opt.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Bearer token only",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    opt.AddSecurityDefinition("Bearer", securityScheme);
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ElectricVehicleDealer API v1");
        
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

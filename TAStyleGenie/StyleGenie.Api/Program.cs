using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using StyleGenie.Api.Author.Service;
using StyleGenie.Application;
using StyleGenie.Application.TryOn;
using StyleGenie.Infrastructure;
using StyleGenie.Infrastructure.Data.Models;
using StyleGenie.Infrastructure.Services;
using System.Text;
using TryOn.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
//builder.Services.AddScoped<IPayOSService, PayOSService>();
builder.Services.AddHttpClient<PayOSService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình Session
builder.Services.AddDistributedMemoryCache(); // Sử dụng bộ nhớ tạm cho session
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true; // Chỉ cho phép truy cập cookie từ server (ngăn chặn XSS)
    options.Cookie.IsEssential = true; // Đảm bảo session hoạt động trong môi trường không có cookie
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn của session
});

builder.Services.AddApplication()
               .AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<ITryOnService, TryOnService>();

// ✅ CORS cho API (chỉ cần cho frontend gọi /api)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AppCors", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// ✅ Thêm cấu hình Authentication dùng JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "super-secret-key"; // bạn có thể lưu vào appsettings.json
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "StyleGenie";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddSwaggerGen(option =>
{
    // Cấu hình tiêu đề cho Swagger UI
    option.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "StyleGenie API", Version = "v1" });

    // 1. Định nghĩa Security Scheme (cách mà API được bảo vệ)
    option.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Vui lòng nhập token với tiền tố 'Bearer ' vào ô bên dưới. Ví dụ: 'Bearer 12345abcdef'",
        Name = "Authorization", // Tên của header
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // 2. Thêm yêu cầu bảo mật, áp dụng scheme "Bearer" đã định nghĩa ở trên
    option.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer" // Phải khớp với tên đã định nghĩa ở trên
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddHostedService<CreditResetService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<EmailOtpService>();
builder.Services.AddScoped<StyleGenie.Application.Wallet.IWalletService, StyleGenie.Infrastructure.Persistence.EfWalletService>();

builder.Services.AddAuthorization(); //

builder.Services.AddAuthorization(options =>
{
    // ✅ Policy cho từng vai trò (dùng claim "role_id" giống JwtTokenHelper)
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("role_id", "1"));

    options.AddPolicy("StaffOnly", policy =>
        policy.RequireClaim("role_id", "2"));

    options.AddPolicy("UserOnly", policy =>
        policy.RequireClaim("role_id", "3"));

    // ✅ Policy kết hợp cho cả Staff và Admin
    options.AddPolicy("StaffOrAdmin", policy =>
        policy.RequireClaim("role_id", "1", "2"));
});





var app = builder.Build();

app.UseCors("AppCors");   // áp dụng cho API
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();


// Cấu hình session middleware (đảm bảo session được sử dụng trước khi xử lý các middleware khác)
app.UseSession();
app.MapControllers();

app.Run();

using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using ProductManagement.Application.Interfaces;
using ProductManagement.Application.Mappings;
using ProductManagement.Domain.Interfaces;
using ProductManagement.Infrastructure.Services;
using ProductManagement.Persistence.Context;
using ProductManagement.Persistence.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Veritabanı Bağlantısı
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2️⃣ Repository Injection
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// 3️⃣ AutoMapper Injection
// GeneralMapping profile'ımızın bulunduğu assembly'yi veriyoruz
builder.Services.AddAutoMapper(typeof(GeneralMapping).Assembly);

// 4️⃣ MediatR Injection
// Application katmanındaki tüm handler'ları bulması için assembly veriyoruz
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GeneralMapping).Assembly));


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()   // Her yerden gelen isteği kabul et
                   .AllowAnyMethod()   // GET, POST, PUT, DELETE hepsine izin ver
                   .AllowAnyHeader();  // Tüm headerlara izin ver
        });
});

// 5️⃣ Controller ekle
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer(); // Endpointleri tarar
builder.Services.AddSwaggerGen();           // Dokümanı oluşturur
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient<ProductManagement.Infrastructure.Services.OpenAiService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProductManagement API", Version = "v1" });

    // Kilit Butonunu Tanımla
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Kilit Butonunu Aktifleştir
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// 7️⃣ Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

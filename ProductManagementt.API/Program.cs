using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Application.Mappings;
using ProductManagement.Domain.Interfaces;
using ProductManagement.Persistence.Context;
using ProductManagement.Persistence.Repositories;

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

// 5️⃣ Controller ekle
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer(); // Endpointleri tarar
builder.Services.AddSwaggerGen();           // Dokümanı oluşturur

var app = builder.Build();

// 7️⃣ Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

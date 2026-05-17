using CarPark.Batch;
using CarPark.Data;
using CarPark.Interfaces;
using CarPark.Repositories;
using CarPark.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=carpark.db"));

// ── Repositories ──────────────────────────────────────────────
builder.Services.AddScoped<ICarParkRepository, CarParkRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserFavouriteRepository, UserFavouriteRepository>();
builder.Services.AddScoped<IBatchJobRepository, BatchJobRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ── Services ──────────────────────────────────────────────────
builder.Services.AddScoped<ICarParkService, CarParkService>();
builder.Services.AddScoped<IFavouriteService, FavouriteService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBatchImportService, BatchImportService>();
builder.Services.AddSingleton<CsvCarParkParser>();

// ── JWT Authentication ────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key must be set.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero
        };

        options.MapInboundClaims = false;
    });

builder.Services.AddAuthorization();

// ── Controllers ───────────────────────────────────────────────
builder.Services.AddControllers();

// ── OpenAPI (built into .NET 10) ──────────────────────────────
builder.Services.AddOpenApi();

var app = builder.Build();

// ── Auto create database on startup ───────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// ── Middleware pipeline ───────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
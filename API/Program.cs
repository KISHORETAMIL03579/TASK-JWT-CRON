using APPLICATION.AUTOMAP;
using AutoMapper;
using APPLICATION.INTERFACE;
using Hangfire;
using Hangfire.PostgreSql;
using INFRASTRUCTURE.AUTH;
using INFRASTRUCTURE.DATA;
using INFRASTRUCTURE.SERVICES;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---------------- CONTROLLERS ----------------
builder.Services.AddControllers();

// ---------------- CACHE ----------------
builder.Services.AddMemoryCache();

// ---------------- DI ----------------
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddAutoMapper(typeof(EmployeeMapping));

// ---------------- SWAGGER ----------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

// ---------------- JWT ----------------
string jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is missing");

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
            Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

// ---------------- DB ----------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ---------------- HANGFIRE ----------------
builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(options =>
    {
        options.UseNpgsqlConnection(
            builder.Configuration.GetConnectionString("DefaultConnection"));
    });
});

builder.Services.AddHangfireServer();

var app = builder.Build();

// ---------------- SWAGGER ----------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ---------------- HANGFIRE DASHBOARD ----------------
app.UseHangfireDashboard("/hangfire");

// ---------------- MIDDLEWARE ----------------
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// ---------------- CRON JOB ----------------
app.Lifetime.ApplicationStarted.Register(() =>
{
    RecurringJob.AddOrUpdate<EmployeeCleanupJob>(
        "employee-cleanup-job",
        job => job.ArchiveEmployees(),
        "*/5 * * * *" // daily at midnight
    );
});

// ---------------- CONTROLLERS ----------------
app.MapControllers();

app.Run();
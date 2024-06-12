using Kitana.Api;
using Kitana.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.OpenApi.Models;
using System.Text;
using Kitana.Service.Services;
using Kitana.Core;
using Kitana.Infrastructure.Repository;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Kitana.Service.Model.ResponseHandlers;
using System.Reflection;
using Kitana.Api.Middleware;
using Scaf.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var envFilePath = builder.Configuration["EnvFilePath"];

DotNetEnv.Env.Load(envFilePath);

var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
builder.Services.AddDbContext<SkillForgeDBContext>(options =>
{
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Kitana.Api"));
});



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter Token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SkillForge", Version = "v1", Description = "[Base URL: api/v1]" });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme{
            Reference = new OpenApiReference{
            Type = ReferenceType.SecurityScheme,
            Id ="Bearer"
            }
        },
        new string[]{ }
        }
    });
});

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
        ValidIssuer = Environment.GetEnvironmentVariable("Jwt_Issuer"),
        ValidAudience = Environment.GetEnvironmentVariable("Jwt_Audience"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("Jwt_Key")))

    };
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddHealthChecks();

var services = builder.Services;
var configuration = builder.Configuration;
DI.ConfigureServices(services, configuration);

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", " V1");
    });
}

app.UseHttpsRedirection();

app.UseSession();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();

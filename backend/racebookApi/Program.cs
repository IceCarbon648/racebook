using AspNet.Security.OAuth.Discord;
using Business.Helpers;
using Helpers.Interfaces;
using Business.Startup;
using CloudinaryDotNet;
using DotNetEnv;
using FluentValidation;
using Infrastructure.Startup;
using MagicBytesValidator.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using racebookApi.Middleware;
using Scalar.AspNetCore;
using Serilog;
using System.Data;
using System.Text;
using Models.Validators.Filter;
using Helpers;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

string discordClientId = Environment.GetEnvironmentVariable("discordClientId")!;
string discordClientSecret = Environment.GetEnvironmentVariable("discordClientSecret")!;
string cloudinaryName = Environment.GetEnvironmentVariable("cloudinaryName")!;
string cloudinaryKey = Environment.GetEnvironmentVariable("cloudinaryKey")!;
string cloudinarySecret = Environment.GetEnvironmentVariable("cloudinarySecret")!;

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddHttpClient("amax-api", client =>
{
    client.BaseAddress = new Uri("https://amax-emu.com/api/");
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddBusiness();
builder.Services.AddInfrastructure();
builder.Services.AddScoped(typeof(ValidationFilter<>));
ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Continue;

builder.Services.AddSingleton<MagicBytesValidator.Services.IValidator, Validator>(sp => {
    Validator validator = new Validator();
    validator.Mapping.Register([new TpfFileType(), new PngFileType(), new JpgFileType()]);
    return validator;
});

builder.Services.AddScoped<IFileChecker, FileChecker>();

builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection"))
);
builder.Services.AddSingleton(provider => new Cloudinary(new Account { ApiKey = cloudinaryKey, ApiSecret = cloudinarySecret, Cloud = cloudinaryName }));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwtOptions =>
{
    jwtOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
})
.AddCookie()
.AddDiscord(options =>
{
    options.ClientId = discordClientId;
    options.ClientSecret = discordClientSecret;
    options.SaveTokens = true;
});

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
});

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(o =>
        o.WithTheme(ScalarTheme.BluePlanet)
    );
}

app.Use(async (context, next) =>
{
    var token = context.Request.Cookies["access_token"];
    if (!string.IsNullOrEmpty(token))
        context.Request.Headers.Authorization = $"Bearer {token}";

    await next();
});

app.UseExceptionHandler(_ => { });
app.UseCors("AllowOrigin");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
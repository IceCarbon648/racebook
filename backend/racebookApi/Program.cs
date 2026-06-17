using AspNet.Security.OAuth.Discord;
using CloudinaryDotNet;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using racebookApi.Repositories;
using racebookApi.Repositories.Interfaces;
using racebookApi.Services;
using racebookApi.Services.Interfaces;
using Scalar.AspNetCore;
using System.Data;
using System.Text;
using AmaxApiAdapter.Startup;

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

builder.Services.AddAmax();

builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection"))
);
builder.Services.AddSingleton(provider => new Cloudinary(new Account { ApiKey = cloudinaryKey, ApiSecret = cloudinarySecret, Cloud = cloudinaryName }));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICloudinaryRepository, CloudinaryRepository>();
builder.Services.AddScoped<IModRepository, ModRepository>();
builder.Services.AddScoped<IModService, ModService>();
builder.Services.AddScoped<IPreviewImageRepository, PreviewImageRepository>();
builder.Services.AddScoped<IUserService, UserService>();


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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(o =>
        o.WithTheme(ScalarTheme.BluePlanet)
    );
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
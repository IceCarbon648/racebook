using AspNet.Security.OAuth.Discord;
using CloudinaryDotNet;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
using racebookApi.Data;
using racebookApi.Repositories;
using racebookApi.Repositories.Interfaces;
using racebookApi.Services;
using racebookApi.Services.Interfaces;
using Scalar.AspNetCore;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

string discordClientId = Environment.GetEnvironmentVariable("discordClientId");
string discordClientSecret = Environment.GetEnvironmentVariable("discordClientSecret");
string cloudinaryName = Environment.GetEnvironmentVariable("cloudinaryName");
string cloudinaryKey = Environment.GetEnvironmentVariable("cloudinaryKey");
string cloudinarySecret = Environment.GetEnvironmentVariable("cloudinarySecret");

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddHttpClient("amax-api", client =>
{
    client.BaseAddress = new Uri("https://amax-emu.com/api/");
});

builder.Services.AddScoped<IDapperContext, DapperContext>();
builder.Services.AddSingleton(provider => new Cloudinary(new Account { ApiKey = cloudinaryKey, ApiSecret = cloudinarySecret, Cloud = cloudinaryName }));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICloudinaryRepository, CloudinaryRepository>();
builder.Services.AddScoped<IModRepository, ModRepository>();
builder.Services.AddScoped<IModService, ModService>();
builder.Services.AddScoped<IPreviewImageRepository, PreviewImageRepository>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
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
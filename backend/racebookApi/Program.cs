using Scalar.AspNetCore;
using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication.Cookies;
using racebookApi.Services.Interfaces;
using racebookApi.Services;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

string clientId = Environment.GetEnvironmentVariable("discordClientId");
string clientSecret = Environment.GetEnvironmentVariable("discordClientSecret");

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddHttpClient("amax-api", client =>
{
    client.BaseAddress = new Uri("https://amax-emu.com/api/");
});

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddDiscord(options =>
{
    options.ClientId = clientId;
    options.ClientSecret = clientSecret;
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
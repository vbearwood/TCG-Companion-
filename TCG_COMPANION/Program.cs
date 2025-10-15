using Microsoft.EntityFrameworkCore;
using TCG_COMPANION.Data;
using TCG_COMPANION.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using DotnetGeminiSDK.Client.Interfaces;
using DotnetGeminiSDK.Client;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<UserContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();

builder.Services.AddDbContext<DeckContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();

builder.Services.AddDbContext<CollectionsContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
    {
        o.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddHttpClient();

builder.Services.AddHttpClient("PokemonTCG", c =>
{
    c.BaseAddress = new Uri("https://api.pokemontcg.io/");
    c.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient("Gemini", c =>
{
    c.BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/");
    c.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddScoped<IDeckStrategyService, DeckStrategyService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllers();

app.Run();


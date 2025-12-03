using Microsoft.EntityFrameworkCore;
using TCG_COMPANION.Data;
using TCG_COMPANION.Utils;
using TCG_COMPANION.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<DeckContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DeckConnection")));

builder.Services.AddDbContext<CollectionsContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("CollectionConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o => o.SlidingExpiration = true);

builder.Services.AddAuthorization();
builder.Services.AddHttpClient();
builder.Services.AddSignalR();

builder.Services.AddHttpClient("PokemonTCG", client =>
{
    client.BaseAddress = new Uri("https://api.pokemontcg.io/v2/");
    client.DefaultRequestHeaders.Add("X-Api-key", builder.Configuration["PokemonTCGApiKey"]);

});

builder.Services.AddHttpClient<GeminiService>();

builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddSingleton<PokemonSetHolder>();

var app = builder.Build();


app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapHub<ChatHub>("chatHub");
app.Run();
using CatBook.API.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddOpenApi();
builder.Services.AddDbContext<CatBookContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddControllers();

//OAuth Google Authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
        ;
    })
    .AddCookie()
    .AddGoogle(options =>
    {
        var googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
        options.ClientId = googleAuthNSection["ClientId"];
        options.ClientSecret = googleAuthNSection["ClientSecret"];;
        options.CallbackPath = "/signin-google";
    });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("login", async context =>
{
    await context.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
    {
        RedirectUri = "/profile"
    });
});
app.MapGet("/profile", [Authorize](HttpContext context) =>
{
    var user = context.User.Identity?.Name;
    return Results.Ok($"Welcome {user}");
});

app.Run();


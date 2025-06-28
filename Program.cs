using CatBook.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration.GetConnectionString("SUPABASE-HOST");

var frontendUrl = "http://localhost:3000" ?? "hosting-connection-url";

builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.Cookie.Name = "CatBookAuthCookie";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // se usi HTTPS
        options.LoginPath = "/auth/google"; // o una pagina login custom
        options.ExpireTimeSpan = TimeSpan.FromDays(15);
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireRole("Admin"));
});




//Database context
builder.Services.AddOpenApi();
builder.Services.AddDbContext<CatBookContext>(options => options.UseNpgsql(connectionString));


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: frontendUrl, policy=> policy.WithOrigins(frontendUrl).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
});

builder.Services.AddControllers();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors(frontendUrl);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();


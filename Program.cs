using CatBook.API.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration.GetConnectionString("SUPABASE-HOST");

var frontendUrl = "http://localhost:3000" ?? "hosting-connction-url";

var googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");


//Database context
builder.Services.AddOpenApi();
builder.Services.AddDbContext<CatBookContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: frontendUrl, policy=> policy.WithOrigins(frontendUrl).AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
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


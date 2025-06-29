using System.Security.Claims;
using CatBook.API.Data;
using CatBook.API.DTOs;
using CatBook.API.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatBook.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly CatBookContext _context;

    public UsersController(IConfiguration config, CatBookContext context)
    {
        _config = config;
        _context = context;
    }

    [HttpGet("loggedUser")]
    public async Task<ActionResult<UserDTO>> GetLoggedUser()
    {
        var googleId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (googleId == null)
            return Unauthorized();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);
        if (user == null)
            return NotFound();

        return Ok(new UserDTO
        {
            DisplayName = user.DisplayName,
            Role = user.Role,
            Profile = user.Profile
        });
    }

    [AllowAnonymous]
    [HttpPost("/auth/google")]
    public async Task<IActionResult> AuthWithGoogle()
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        if (!authHeader.StartsWith("Bearer "))
            return Unauthorized();

        var token = authHeader["Bearer ".Length..];

        var payload = await GoogleJsonWebSignature.ValidateAsync(token, new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { _config["Authentication:Google:ClientId"] }
        });

        var user = await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == payload.Subject);
        if (user == null)
        {
            user = new User
            {
                GoogleId = payload.Subject,
                Email = payload.Email,
                DisplayName = payload.Name,
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.GoogleId),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role ?? "User")
        };

        var identity = new ClaimsIdentity(claims, "Google");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(principal);

        return Ok();
    }

    [HttpPatch("me")]
    public async Task<ActionResult> PatchUser([FromBody] UserDTO dto)
    {
        var googleId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (googleId == null)
            return Unauthorized();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);
        if (user == null)
            return NotFound();

        if (!string.IsNullOrEmpty(dto.DisplayName))
            user.DisplayName = dto.DisplayName;

        if (!string.IsNullOrEmpty(dto.Role))
            user.Role = dto.Role;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser()
    {
        var googleId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);

        if (user == null)
            return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
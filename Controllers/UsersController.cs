using System.Security.Claims;
using CatBook.API.Data;
using CatBook.API.DTOs;
using CatBook.API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatBook.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly CatBookContext _context;

    public UsersController(CatBookContext context) { _context = context; }

    [AllowAnonymous]
    [HttpGet("/auth/google-login")]
    public async Task<ActionResult<User>> AuthLogin()
    {
        var auth = new AuthenticationProperties()
        {
            RedirectUri = "/profile"
        };
        return Challenge(auth, GoogleDefaults.AuthenticationScheme);
    }

    //testing endpoint
    [HttpGet("/profile")]
    public IActionResult Profile()
    {
        var name = User.Identity?.Name;
        var email = User.FindFirstValue(ClaimTypes.Email);
        
        return Ok(new {name, email});
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser()
    {
        var googleId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var displayName = User.FindFirstValue(ClaimTypes.Name);
        if (googleId == null)
            return Unauthorized();
        
        var newUser = new User
        {
            GoogleId = googleId,
            Email = email,
            DisplayName = displayName
        };
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        return Ok(newUser);
    }

    [HttpPatch("me")]
    public async Task<ActionResult<User>> PatchUser([FromBody] UserDTO dto)
    {
        var googleId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (googleId == null)
            return Unauthorized();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);
        if (user == null)
            return NotFound();
        if(!string.IsNullOrEmpty(dto.DisplayName))
            user.DisplayName = dto.DisplayName;
        if(!string.IsNullOrEmpty(dto.Role))
            user.Role = dto.Role;
        
        await _context.SaveChangesAsync();
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete]
    public async Task<ActionResult<User>> DeleteUser()
    {
        var existingUser =
            await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (existingUser == null)
        {
            return NotFound();
        }

        _context.Users.Remove(existingUser);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
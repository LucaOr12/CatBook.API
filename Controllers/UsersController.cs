using System.Security.Claims;
using CatBook.API.Data;
using CatBook.API.DTOs;
using CatBook.API.Models;
using Google.Apis.Auth;
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

    [AllowAnonymous]
    [HttpPost("/auth/google")]
    public async Task<ActionResult<User>> AuthWithGoogle()
    {
        var authHeader = Request.Headers["Authorization"].ToString();;
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
        return Ok(user);
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
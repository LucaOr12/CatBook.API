using System.Security.Claims;
using CatBook.API.Data;
using CatBook.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatBook.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CatProfilesController : ControllerBase
{
    private readonly CatBookContext _context;

    public CatProfilesController(CatBookContext context)
    {
        _context = context;
    }

    [HttpGet("me")]
    public async Task<ActionResult<CatProfile>> GetMyProfile()
    {
        var googleId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (googleId == null) return Unauthorized();
        
        var user = await _context.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.GoogleId == googleId);
        if(user == null) return NotFound();
        return Ok(user.Profile);
    }


    [HttpPost]
    public async Task<ActionResult<CatProfile>> CreateProfile([FromBody] CatProfile profile)
    {
        var googleId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (googleId == null) return Unauthorized();
        
        var user = await _context.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.GoogleId == googleId);
        if(user == null) return NotFound();
        if(user.Profile != null) return BadRequest("User already has a Cat profile");
        
        user.Profile = profile;
        
        profile.UserId = user.Id;
        await _context.SaveChangesAsync();
        return Ok(profile);
    }
    
    [HttpPatch]
    public async Task<ActionResult<CatProfile>> PatchProfile([FromBody] CatProfile updated)
    {
        var googleId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (googleId == null) return Unauthorized();
        
        var user = await _context.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.GoogleId == googleId);
        if(user == null) return NotFound();
        
        user.Profile.CatName = updated.CatName ?? user.Profile.CatName;
        user.Profile.Age = updated.Age ?? user.Profile.Age;
        user.Profile.Breed = updated.Breed ?? user.Profile.Breed;
        user.Profile.Bio = updated.Bio ?? user.Profile.Bio;
        
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    
    [HttpDelete]
    public async Task<ActionResult<CatProfile>> DeleteProfile()
    {
        var googleId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (googleId == null) return Unauthorized();
        
        var user = await _context.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.GoogleId == googleId);
        if(user == null) return NotFound();
        
        _context.CatProfiles.Remove(user.Profile);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
}



    
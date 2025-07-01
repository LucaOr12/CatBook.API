using System.Security.Claims;
using CatBook.API.Data;
using CatBook.API.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CatBook.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly CatBookContext _context;
    
    public PostsController(CatBookContext context)
    {
        _context = context;
    }
    
    [HttpGet("me")]
    public async Task<ActionResult<IEnumerable<Post>>> MyPosts()
    {
        var googleId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (googleId == null) return Unauthorized();
        
        var user = await _context.Users
            .Include(u => u.Profile)
            .ThenInclude(p => p.Posts)
            .FirstOrDefaultAsync(u => u.GoogleId == googleId);
        
        if(user?.Profile?.Posts == null) return Ok(new List<Post>());
        
        return Ok(user.Profile.Posts.OrderByDescending(p => p.PostedAt).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<Post>> CreatePost([FromBody] Post post)
    {
        var googleId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (googleId == null) return Unauthorized();
        
        var user = await _context.Users.Include(u => u.Profile).ThenInclude(p => p.Posts).FirstOrDefaultAsync(u => u.GoogleId == googleId);
        
        if(user?.Profile == null)
            return BadRequest("User does not have a profile");
        
        post.PostedAt = DateTime.UtcNow;
        post.Likes = 0;
        post.Comments = new List<string>();
        
        post.CatProfileId = user.Profile.Id;
        post.CatName = user.Profile.CatName;
        
        post.CatProfile = null;
        
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        return Ok(post);
    }
}
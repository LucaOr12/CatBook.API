using System.Security.Claims;
using CatBook.API.Data;
using CatBook.API.DTOs;
using CatBook.API.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CatBook.API.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly CatBookContext _context;
    
    public PostsController(CatBookContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    [HttpHead("ping")]
    public IActionResult Ping() => Ok("I'm Alive!");

    [AllowAnonymous]
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Post>>> GetAllPosts()
    {
        var posts = await _context.Posts.OrderByDescending(p => p.PostedAt)
            .Select(p => new Post
            {
                Caption = p.Caption,
                ImageUrl = p.ImageUrl,
                PostedAt = p.PostedAt,
                Likes = p.Likes,
                Comments = p.Comments,
                CatName = p.CatName
            }).ToListAsync();
        return Ok(posts);
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
    public async Task<ActionResult<Post>> CreatePost([FromBody] PostDTO dto)
    {
        var googleId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (googleId == null) return Unauthorized();
        
        var user = await _context.Users.Include(u => u.Profile).ThenInclude(p => p.Posts).FirstOrDefaultAsync(u => u.GoogleId == googleId);
        
        if(user?.Profile == null)
            return BadRequest("User does not have a profile");

        var post = new Post
        {
            Caption = dto.Caption,
            ImageUrl = dto.ImageUrl,
            PostedAt = DateTime.UtcNow,
            Likes = 0,
            Comments = new List<string>(),
            CatProfileId = user.Profile.Id,
            CatName = user.Profile.CatName
        };
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        
        return Ok(post);
    }
}
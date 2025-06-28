using System.Security.Claims;
using CatBook.API.Data;
using CatBook.API.Models;
using Microsoft.AspNetCore.Authorization;
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
}
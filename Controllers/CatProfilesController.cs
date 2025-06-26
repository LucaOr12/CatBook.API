using CatBook.API.Data;
using CatBook.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatBook.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CatProfilesController : ControllerBase
{
    private readonly CatBookContext _context;

    public CatProfilesController(CatBookContext context)
    {
        _context = context;
    }
    
}
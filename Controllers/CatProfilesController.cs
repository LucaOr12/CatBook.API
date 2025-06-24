using CatBook.API.Data;
using CatBook.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatBook.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CatProfilesController : ControllerBase
{
    private readonly CatBookContext _context;

    public CatProfilesController(CatBookContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<CatProfile>>> GetAll()
    {
        var profiles = await _context.CatProfiles.ToListAsync();
        return Ok(profiles);
    }

    [HttpGet("{id}", Name = "GetCatProfile")]
    public async Task<ActionResult<CatProfile>> Get(int id)
    {
        var profile = await _context.CatProfiles.FindAsync(id);
        return profile == null ? NotFound() : Ok(profile);
    }

    [HttpPost]
    public async Task<ActionResult<CatProfile>> Post([FromBody] CatProfile profile)
    {
       _context.CatProfiles.Add(profile);
       await _context.SaveChangesAsync();
       return CreatedAtRoute("GetCatProfile", new { id = profile.Id }, profile);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] CatProfile profile)
    {
        if (id != profile.Id)
        {
            return BadRequest("Id mismatch");
        }
        var exists = await _context.CatProfiles.AnyAsync(c => c.Id == id);
        if (!exists)
        {
            return NotFound();
        }
        _context.Entry(profile).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.CatProfiles.AnyAsync(c => c.Id == id))
            {
                return NotFound();
            }
            else throw;
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var profile = await _context.CatProfiles.FindAsync(id);

        if (profile == null)
        {
            return NotFound();
        }
        _context.CatProfiles.Remove(profile);

        await _context.SaveChangesAsync();
        return NoContent();
    }
}
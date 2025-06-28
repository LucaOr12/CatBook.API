using CatBook.API.Models;
using Microsoft.EntityFrameworkCore;


namespace CatBook.API.Data;

public class CatBookContext : DbContext
{
    public CatBookContext(DbContextOptions<CatBookContext> options) : base(options)
    {
    }
    public DbSet<CatProfile> CatProfiles { get; set; }
    public DbSet<User> Users { get; set; }
    
    public DbSet<Post> Posts { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace CatBook.API.Models;

public class User
{
    public int Id { get; set; }
    
    //Google info
    [Required]
    public string GoogleId { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string DisplayName { get; set; }
    
    //app info
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public string Role { get; set; } = "User";
    
    //Cat Profile Relation
    public CatProfile? Profile { get; set; }
}
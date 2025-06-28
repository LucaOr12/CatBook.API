using System.ComponentModel.DataAnnotations;

namespace CatBook.API.Models;

public class CatProfile
{
    public int  Id { get; set; }
    
    //content
    [Required]
    public string CatName { get; set; }
    [Required]
    public string Breed { get; set; }
    [Required]
    public int? Age { get; set; }
    [Required]
    public string Bio { get; set; }
    
    public List<Post> Posts { get; set; } 
    
    //foreign Key
    public int UserId { get; set; }
    public User User { get; set; }
}
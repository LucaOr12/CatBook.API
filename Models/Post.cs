using System.ComponentModel.DataAnnotations;

namespace CatBook.API.Models;

public class Post
{
    public int Id { get; set; }
    [Required]
    public string ImageUrl { get; set; }
    [Required]
    public string Caption { get; set; }
    [Required]
    public DateTime PostedAt { get; set; } = DateTime.UtcNow;
    
    //Post Interaction
    public int? Likes { get; set; }
    public List<string>? Comments { get; set; }
    
    //foreign Key
    public int CatProfileId { get; set; }
    public CatProfile CatProfile { get; set; }
}
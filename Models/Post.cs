using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CatBook.API.Models;

public class Post
{
    public int Id { get; set; }
    
    //Post Content
    public string CatName { get; set; }
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
    [JsonIgnore]
    public CatProfile? CatProfile { get; set; }
}
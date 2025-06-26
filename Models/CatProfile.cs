using System.ComponentModel.DataAnnotations;

namespace CatBook.API.Models;

public class CatProfile
{
    public int  Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Breed { get; set; }
    [Required]
    public int Age { get; set; }
    [Required]
    public string Bio { get; set; }
}
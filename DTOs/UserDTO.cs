namespace CatBook.API.DTOs;

public class UserDTO
{
    public string? DisplayName { get; set; }
    public string? Role { get; set; } = "User";
}
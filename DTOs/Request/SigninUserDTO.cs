using System.ComponentModel.DataAnnotations;

namespace todo_application.DTOs.Request;

public class SigninUserDTO
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}
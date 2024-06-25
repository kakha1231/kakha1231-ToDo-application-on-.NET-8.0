using todo_application.Models;

namespace todo_application.DTOs.Response;

public class GetUserDTO
{
    public string Username { get; set; }
    public string Email { get; set; }
    
    public List<TaskItem>? TaskItems { set; get; }
}
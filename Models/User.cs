using Microsoft.AspNetCore.Identity;

namespace todo_application.Models;

public class User : IdentityUser
{
    public ICollection<TaskItem>? Tasks { set; get; }
}
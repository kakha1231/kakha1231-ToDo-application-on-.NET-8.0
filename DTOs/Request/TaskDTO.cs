namespace todo_application.DTOs.Request;

public class TaskDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }

}
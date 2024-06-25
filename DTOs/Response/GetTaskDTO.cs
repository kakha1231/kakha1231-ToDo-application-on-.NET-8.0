namespace todo_application.DTOs.Response;

public class GetTaskDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
}
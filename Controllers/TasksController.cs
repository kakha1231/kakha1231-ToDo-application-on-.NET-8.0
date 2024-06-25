using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todo_application.Data;
using todo_application.DTOs.Request;
using todo_application.DTOs.Response;
using todo_application.Models;

namespace todo_application.Controllers;

[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/[controller]")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public TasksController(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    

    [HttpGet("/tasks")]
    public async Task<IActionResult> GetTasks()
    {
        var userid = User.Claims.First(user => user.Type == "Id").Value;
       
        if (string.IsNullOrEmpty(userid))
        {
            return Unauthorized();
        }
        
        var tasks = await _context.TaskItems.Where(t => t.UserId == userid)
            .Select(t => new GetTaskDTO
            {
                Name = t.Name,
                Description = t.Description,
                IsCompleted = t.IsCompleted
            }).ToListAsync();
        
        return Ok(tasks);
    }

    [HttpPost("/addtask")]
    public async Task<IActionResult> CreateTask([FromBody] TaskDTO taskDto)
    {
        var userid = User.Claims.First(user => user.Type == "Id").Value;
        
        if (string.IsNullOrEmpty(userid))
        {
            return Unauthorized();
        }
        
        var task = new TaskItem
        {
            Name = taskDto.Name,
            Description = taskDto.Description,
            IsCompleted = taskDto.IsCompleted,
            UserId = userid
        };

        _context.TaskItems.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, task);
    }

    [HttpPut("/update{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskDTO taskDto)
    {
        var task = await _context.TaskItems.FindAsync(id);

        if (task == null)
        {
            return NotFound();
        }

        var userid = User.Claims.First(user => user.Type == "Id").Value;
        if (task.UserId != userid)
        {
            return Unauthorized();
        }
        
        task.Name = taskDto.Name;
        task.Description = taskDto.Description;
        task.IsCompleted = taskDto.IsCompleted;
        
        _context.Entry(task).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("/delete{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.TaskItems.FindAsync(id);

        if (task == null)
        {
            return NotFound();
        }

        var userid = User.Claims.First(User => User.Type == "Id").Value;
        if (task.UserId != userid)
        {
            return Unauthorized();
        }

        _context.TaskItems.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
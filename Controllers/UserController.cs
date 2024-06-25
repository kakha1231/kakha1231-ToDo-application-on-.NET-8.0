using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todo_application.Data;
using todo_application.DTOs.Request;
using todo_application.DTOs.Response;
using todo_application.JWT;
using todo_application.Models;

namespace todo_application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly JwtService _jwtService;
    private readonly AppDbContext _appDbContext;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, JwtService jwtService, AppDbContext appDbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _appDbContext = appDbContext;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]CreateUserDTO userDto)
    {
        var user = new User()
        {
            UserName = userDto.Username,
            Email = userDto.Email,
        };
        var result = await _userManager.CreateAsync(user, userDto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(new { message = "User registered successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]SigninUserDTO userDto)
    {
        var user = await _userManager.FindByNameAsync(userDto.Username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, userDto.Password))
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        var token = _jwtService.CreateJwt(user);

        return Ok(new
        {
            message = "Login successful",
            JWT = token
        });
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return Ok(new { message = "Logout successful" });
    }
    
    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet("/checkuser")]
    public async Task<IActionResult> GetUser()
    {
        try
        {
            var userid = User.Claims.First(claim => claim.Type == "Id").Value;
            var user = await _userManager.FindByIdAsync(userid);

            var getuser = new GetUserDTO()
            {
                Username = user.UserName,
                Email = user.Email,
                TaskItems = await _appDbContext.TaskItems.Where(item => item.UserId == userid).ToListAsync()
            };
            
            return Ok(getuser);
        }
        catch
        {
            return Unauthorized();
        }
    }
}
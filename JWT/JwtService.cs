using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using todo_application.Models;

namespace todo_application.JWT;

public class JwtService
{
    private readonly IConfiguration _config;
    
    public JwtService(IConfiguration config)
    {
        _config = config;
    }
    public string CreateJwt(User user)
    {
        var userClaims = new List<Claim>  
        {
            new Claim("Id", user.Id),
            new Claim("Email", user.Email)
        };
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var sectoken = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            userClaims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials);
        
        var token = new JwtSecurityTokenHandler().WriteToken(sectoken);

        return token;
    }
}
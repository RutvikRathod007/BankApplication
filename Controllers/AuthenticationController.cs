using BankApplication.Managers.AuthenticationManager;
using BankApplication.Models.Dtos.UsersDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankApplication.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController:ControllerBase
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IConfiguration _config;

        public AuthenticationController(IAuthenticationManager authenticationManager,IConfiguration config)
        {
            _authenticationManager = authenticationManager;
            _config = config;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto loginData)
        {

            var res = await _authenticationManager.Login(loginData);
            if(res.Success && res.Data!=null)
            {
              
               
                
                    var token = GenerateToken(loginData,res.Data.Role);
                    return Ok(new {token,userName=res.Data.Username,role=res.Data.Role,Success=true});
                
             
            }
            return BadRequest(new { Success = false });
          
        }
        private string GenerateToken(UserLoginDto user,string role)
        {
             
            var claims = new[] { new Claim(ClaimTypes.Role,role) };
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], claims: claims, expires: DateTime.Now.AddSeconds(1), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserSignUpDto signUpData)
        {
            var res = await _authenticationManager.Register(signUpData);
            return Ok(res);
        }
    }
}

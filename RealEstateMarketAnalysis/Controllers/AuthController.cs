using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RealEstateMarketAnalysis.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealEstateMarketAnalysis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static UserModel user = new UserModel();
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;

        }


        [HttpPost("register")]
        public ActionResult<UserModel> Register([FromBody] UserDto userDto)
        {
            user.Email = userDto.Email;
            user.Password = userDto.Password;
            return Ok(user);
        }
        [HttpPost("login")]
        public ActionResult<UserModel> Login([FromBody] UserDto userDto)
        {
            if (user.Email != userDto.Email || user.Password != userDto.Password)
            {
                return BadRequest(new { Message = "Invalid email or password" });
            }
            {

                string token = GenerateToken(user);

                return Ok(token);



            }
        }


        private string GenerateToken(UserModel user)
        {
            List<Claim> claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Name),
    };

            string? tokenKey = _configuration.GetSection("AppSettings:Token").Value;

            if (string.IsNullOrEmpty(tokenKey) || tokenKey.Length < 16)
            {
                throw new ArgumentException("JWT ключ отсутствует или слишком короткий. Минимум 16 символов.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}


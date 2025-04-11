using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RealEstateMarketAnalysis.Data;
using RealEstateMarketAnalysis.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<UserModel>> Register([FromBody] UserDto userDto, [FromServices] DataContext dbContext)
        {
            try
            {
                if (!IsValidEmail(userDto.Email))
                    return BadRequest("Invalid email or password");

                if (string.IsNullOrWhiteSpace(userDto.Name) || userDto.Name.Length <= 3)
                    return BadRequest("Invalid email or password");

                if (string.IsNullOrWhiteSpace(userDto.Password) || userDto.Password.Length < 6)
                    return BadRequest("Invalid email or password");

                // Проверяем, есть ли уже такой Email в базе
                if (await dbContext.Users.AnyAsync(u => u.Email == userDto.Email))
                    return BadRequest("Invalid email or password.");

                // Хешируем пароль
              

                var user = new UserModel
                {
                    Email = userDto.Email,
                    Name = userDto.Name,
                    Password = userDto.Password, 
                };

                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();

                return Ok(user);
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                Console.WriteLine($"Ошибка: {ex.Message}");
                return StatusCode(500, $"Ошибка сервера: {ex.Message}");
            }
        }


        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] UserDto userDto, [FromServices] DataContext dbContext)
        {
            // Получаем пользователя из базы данных по email
            var user = await dbContext.Users
                                       .FirstOrDefaultAsync(u => u.Email == userDto.Email);

            if (user == null)
            {
                return BadRequest(new { Message = "Invalid email or password" });
            }

            // Проверка пароля
            if (user.Password != userDto.Password)
            {
                return BadRequest(new { Message = "Invalid email or password" });
            }

            // Генерация JWT токена
            string token = GenerateToken(user);

            return Ok(new { Token = token });
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


        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

    }

}


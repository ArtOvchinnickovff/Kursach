using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketAnalysis.Data;
using RealEstateMarketAnalysis.Models;
using Microsoft.EntityFrameworkCore;

namespace RealEstateMarketAnalysis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly DataContext _context;

        public FavoritesController(DataContext context)
        {
            _context = context;
        }

        private async Task<UserModel?> GetUser(string email, string password)
        {
            return await _context.Users
                .Include(u => u.Favorites)
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToFavorites(
            [FromQuery] string email,
            [FromQuery] string password,
            [FromBody] FavoriteListingDTO listingDto)
              {
            // Проверка авторизации
            var user = await GetUser(email, password);
            if (user is null)
                return Unauthorized("Неверная почта или пароль");

            // Проверка на дубликаты
            if (user.Favorites.Any(f => f.Url == listingDto.Url))
                return BadRequest("Объект уже в избранном");

            // Создание новой записи
            var listing = new FavoriteListing
            {
                Url = listingDto.Url,
                Title = "Название не указано", 
                Price = "Цена не указана",    
                Address = "Адрес не указан",  
                UserId = user.Id              
            };

            _context.FavoriteListings.Add(listing);
            await _context.SaveChangesAsync();

            return Ok("Добавлено в избранное");
         }

        [HttpDelete("remove/{listingId}")]
        public async Task<IActionResult> RemoveFromFavorites([FromRoute] int listingId)
        {
            var listing = await _context.FavoriteListings
                .FirstOrDefaultAsync(f => f.Id == listingId);

            if (listing is null)
                return NotFound($"Объявление с ID {listingId} не найдено в избранном");

            _context.FavoriteListings.Remove(listing);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Удалено из избранного",
                DeletedId = listingId
            });
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetFavorites([FromQuery] string email, [FromQuery] string password)
        {
            var user = await GetUser(email, password);
            if (user is null) return Unauthorized("Неверная почта или пароль");

            return Ok(user.Favorites);
        }
    }
}
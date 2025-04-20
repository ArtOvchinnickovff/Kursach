using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateMarketAnalysis.Data;
using RealEstateMarketAnalysis.Models;
using System.Security.Claims;

namespace RealEstateMarketAnalysis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly DataContext _context;

        public FavoritesController(DataContext context)
        {
            _context = context;
        }

        private async Task<UserModel?> GetCurrentUser()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return null;

            return await _context.Users
                .Include(u => u.Favorites)
                .FirstOrDefaultAsync(u => u.Email == userEmail);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToFavorites([FromBody] FavoriteListingDTO listingDto)
        {
            var user = await GetCurrentUser();
            if (user is null)
                return Unauthorized("Пользователь не найден");

            if (user.Favorites.Any(f =>
                f.Url == listingDto.Url &&
                f.Title == listingDto.Title &&
                f.Address == listingDto.Address))
            {
                return BadRequest("Этот объект уже в избранном");
            }

            var listing = new FavoriteListing
            {
                Url = listingDto.Url,
                Title = listingDto.Title,
                Price = listingDto.Price,
                Address = listingDto.Address,
                UserId = user.Id
            };

            _context.FavoriteListings.Add(listing);
            await _context.SaveChangesAsync();

            // Возвращаем DTO вместо полной модели
            return Ok(new
            {
                Message = "Добавлено в избранное",
            });
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromFavorites([FromBody] FavoriteListingDTO listingDto)
        {
            var user = await GetCurrentUser();
            if (user is null)
                return Unauthorized("Пользователь не найден");

            // Ищем по всем трём параметрам
            var listing = await _context.FavoriteListings
                .FirstOrDefaultAsync(f =>
                    f.UserId == user.Id &&
                    f.Url == listingDto.Url &&
                    f.Title == listingDto.Title &&
                    f.Address == listingDto.Address);

            if (listing is null)
                return NotFound("Объект не найден в вашем избранном");

            _context.FavoriteListings.Remove(listing);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Удалено из избранного",
                RemovedListing = listingDto
            });
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetFavorites()
        {
            var user = await GetCurrentUser();
            if (user is null)
                return Unauthorized("Пользователь не найден");

            return Ok(user.Favorites);
        }
    }
}
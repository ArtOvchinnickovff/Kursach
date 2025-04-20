using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketAnalysis.Models;
using RealEstateMarketAnalysis.Parser;

namespace RealEstateMarketAnalysis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParserController : ControllerBase
    {
        private readonly CianParserService _cianParserService;

        public ParserController(CianParserService cianParserService)
        {
            _cianParserService = cianParserService;
        }

        [Authorize]
        [HttpGet("parse")]
        public async Task<IActionResult> Parse([FromQuery] CianFilterOptions filter)
        {
            if (string.IsNullOrWhiteSpace(filter?.Location) || filter.MinPrice == null || filter.MaxPrice == null)
                return BadRequest("Please enter data");
            try
            {
                var listings = await _cianParserService.ParseAsync(filter);
                return Ok(listings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка парсинга: {ex.Message}");
            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        // Метод для парсинга данных с сайта Cian и возврата их
        [HttpGet("parse")]
        public async Task<IActionResult> Parse()
        {
            var listings = await _cianParserService.ParseAsync();
            return Ok(listings);
        }
    }
}

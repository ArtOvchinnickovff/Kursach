using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealEstateMarketAnalysis.Models;
using RealEstateMarketAnalysis.Data;
using System.Collections.Generic;
using Microsoft.Playwright;


namespace RealEstateMarketAnalysis.Parser
{
    public class CianParserService
    {
        private readonly DataContext _context;
        private readonly CianHtmlParser _htmlParser;

        public CianParserService(DataContext context, CianHtmlParser htmlParser)
        {
            _context = context;
            _htmlParser = htmlParser;
        }

        // Парсинг HTML с помощью Playwright
        public async Task<List<CianListing>> ParseAsync(CianFilterOptions? filter = null)
        {
            // Если фильтр не передан — используем дефолтные значения
            filter ??= new CianFilterOptions();

            // Формируем URL с параметрами
            var url = BuildCianUrl(filter);

            // Парсим через Playwright
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
            var page = await browser.NewPageAsync();

            await page.GotoAsync(url);
            await page.WaitForSelectorAsync("article[data-name='CardComponent']");

            var html = await page.ContentAsync();
            return _htmlParser.ParseHtml(html);
        }

        private string BuildCianUrl(CianFilterOptions filter)
        {
            var baseUrl = "https://www.cian.ru/cat.php?";
            var queryParams = new List<string>
    {
        $"deal_type={filter.DealType}",
        $"offer_type={filter.PropertyType}",
        "engine_version=2",
        "region=1" // 1 — Москва, 2 — СПб и т.д.
    };

            if (filter.MinPrice.HasValue)
                queryParams.Add($"minprice={filter.MinPrice}");

            if (filter.MaxPrice.HasValue)
                queryParams.Add($"maxprice={filter.MaxPrice}");

            if (filter.RoomsCount.HasValue)
                queryParams.Add($"room{filter.RoomsCount}=1");

            if (!string.IsNullOrEmpty(filter.District))
                queryParams.Add($"district_name={Uri.EscapeDataString(filter.District)}");

            return baseUrl + string.Join("&", queryParams);
        }

        // Импорт в БД
        public async Task ImportListingsAsync()
        {
            var listings = await ParseAsync();
            _context.CianListings.AddRange(listings);
            await _context.SaveChangesAsync();
        }
    }
}
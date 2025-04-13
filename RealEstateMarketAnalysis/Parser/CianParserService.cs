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
        public async Task<List<CianListing>> ParseAsync()
        {
            var url = "https://www.cian.ru/cat.php?deal_type=sale&engine_version=2&offer_type=flat&region=1";

            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
            var page = await browser.NewPageAsync();

            await page.GotoAsync(url);
            await page.WaitForSelectorAsync("article[data-name='CardComponent']");

            var html = await page.ContentAsync();
            return _htmlParser.ParseHtml(html);
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
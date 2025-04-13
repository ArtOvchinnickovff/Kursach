﻿using HtmlAgilityPack;
using RealEstateMarketAnalysis.Models;

namespace RealEstateMarketAnalysis.Parser
{
    public class CianHtmlParser
    {
        public List<CianListing> ParseHtml(string html)
        {
            var listings = new List<CianListing>();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var cards = doc.DocumentNode.SelectNodes("//article[contains(@data-name, 'CardComponent')]");
            if (cards == null) return listings;

            foreach (var card in cards)
            {
                var titleNode = card.SelectSingleNode(".//span[contains(@data-mark, 'Title')]");
                var priceNode = card.SelectSingleNode(".//span[contains(@data-mark, 'MainPrice')]");
                var addressNode = card.SelectSingleNode(".//div[contains(@data-name, 'GeoLabel')]");
                var linkNode = card.SelectSingleNode(".//a[contains(@href, '/sale/')]");

                listings.Add(new CianListing
                {
                    Title = titleNode?.InnerText.Trim() ?? "—",
                    Price = priceNode?.InnerText.Trim() ?? "—",
                    Address = addressNode?.InnerText.Trim() ?? "—",
                    Url = linkNode?.GetAttributeValue("href", "") ?? ""
                });
            }

            return listings;
        }
    }
}

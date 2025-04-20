namespace RealEstateMarketAnalysis.Models
{
    public class FavoriteListingDTO
    {
        public string Url { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Price { get; set; }
        public string Address { get; set; } = null!;
    }
}

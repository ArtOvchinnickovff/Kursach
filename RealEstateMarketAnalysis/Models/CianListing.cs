namespace RealEstateMarketAnalysis.Models
{
    public class CianListing
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Price { get; set; }
        public string Address { get; set; }
        public string Url { get; set; }
        public DateTime ImportedAt { get; set; } = DateTime.UtcNow;

    }
}

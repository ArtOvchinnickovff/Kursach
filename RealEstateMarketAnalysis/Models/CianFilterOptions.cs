namespace RealEstateMarketAnalysis.Models
{
    public class CianFilterOptions
    {
       public string? DealType { get; set; } = "sale"; // sale, rent
        public string? PropertyType { get; set; } = "flat"; // flat, house, etc.
        public string? Location { get; set; } // Москва, СПб и т.д.
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? RoomsCount { get; set; } // 1, 2, 3...
        public string? District { get; set; }
    }
}

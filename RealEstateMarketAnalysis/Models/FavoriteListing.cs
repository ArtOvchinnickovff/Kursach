namespace RealEstateMarketAnalysis.Models
{
    public class FavoriteListing
    {
        public int Id { get; set; }

        public string Url { get; set; } = null!;
        public string Title { get; set; } 
        public string Price { get; set; } 

        // внешний ключ
        public int UserId { get; set; }

        // навигационное свойство
        public UserModel User { get; set; } = null!;
    }
}

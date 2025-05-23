﻿namespace RealEstateMarketAnalysis.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // , "Admin", "User"


        public ICollection<FavoriteListing> Favorites { get; set; } = new List<FavoriteListing>();

    }
}

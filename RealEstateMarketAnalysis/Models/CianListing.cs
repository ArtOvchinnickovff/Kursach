using System.Text.Json;
using System.Text.Json.Serialization;

namespace RealEstateMarketAnalysis.Models
{
    public class CianListing
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Price { get; set; }
        public string Url { get; set; }

        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime ImportedAt { get; set; } = DateTime.UtcNow;
    }

    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateTime.Parse(reader.GetString());

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss"));
    }
}
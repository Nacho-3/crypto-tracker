namespace CryptoTrackerAPI.Models
{
    public class CriptoYaResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("ask")]
        public decimal Ask { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("bid")]
        public decimal Bid { get; set; }
    }
}
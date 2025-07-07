using System.Text.Json.Serialization;

namespace CheapSharkClient.Models
{
    public class Game
    {
        [JsonPropertyName("cheapest")]
        public string Cheapest { get; set; } = "";
        
        [JsonPropertyName("external")]
        public string External { get; set; } = "";
        
        [JsonPropertyName("gameID")]
        public string GameID { get; set; } = "";
        
        [JsonPropertyName("steamAppID")]
        public string SteamAppID { get; set; } = "";
        
        [JsonPropertyName("thumb")]
        public string Thumb { get; set; } = "";
    }
}

using System.Text.Json.Serialization;

namespace CheapSharkClient.Models
{
    public class Deal
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = "";
        
        [JsonPropertyName("salePrice")]
        public string SalePrice { get; set; } = "";
        
        [JsonPropertyName("normalPrice")]
        public string NormalPrice { get; set; } = "";
        
        [JsonPropertyName("savings")]
        public string Savings { get; set; } = "";
        
        [JsonPropertyName("storeID")]
        public string StoreID { get; set; } = "";
        
        [JsonPropertyName("dealID")]
        public string DealID { get; set; } = "";
        
        [JsonPropertyName("thumb")]
        public string Thumb { get; set; } = "";
        
        public decimal SalePriceDecimal => decimal.TryParse(SalePrice, out var price) ? price : 0;
        public decimal SavingsDecimal => decimal.TryParse(Savings, out var savings) ? savings : 0;
    }
}

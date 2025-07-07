using System.Text.Json;
using CheapSharkClient.Models;
using System.Text.Json.Serialization;

namespace CheapSharkClient.Services
{
    public class CheapSharkService
    {
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, Store> _stores = new();

        public CheapSharkService()
        {
            _httpClient = new HttpClient();
            LoadStoresAsync().Wait();
        }

        private async Task LoadStoresAsync()
        {
            try
            {
                string json = await _httpClient.GetStringAsync("https://www.cheapshark.com/api/1.0/stores");
                var stores = JsonSerializer.Deserialize<List<Store>>(json);
                
                if (stores != null)
                {
                    foreach (var store in stores)
                    {
                        _stores[store.StoreID] = store;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось загрузить список магазинов: {ex.Message}");
            }
        }

        public async Task<List<Game>> SearchGamesAsync(string keyword, int limit = 10)
        {
            string url = $"https://www.cheapshark.com/api/1.0/games?title={keyword}&limit={limit}";
            
            try
            {
                string json = await _httpClient.GetStringAsync(url);
                return JsonSerializer.Deserialize<List<Game>>(json) ?? new List<Game>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при поиске игр: {ex.Message}");
                return new List<Game>();
            }
        }

        public async Task<List<GameDeal>> GetDealsAsync(string gameId)
        {
            string url = $"https://www.cheapshark.com/api/1.0/games?id={gameId}";
            
            try
            {
                string json = await _httpClient.GetStringAsync(url);
                var gameInfo = JsonSerializer.Deserialize<GameInfo>(json);
                return gameInfo?.Deals ?? new List<GameDeal>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении сделок: {ex.Message}");
                return new List<GameDeal>();
            }
        }

        public async Task<List<Deal>> GetBestDealsAsync(int limit = 20)
        {
            string url = $"https://www.cheapshark.com/api/1.0/deals?pageSize={limit}&sortBy=Savings";
            
            try
            {
                string json = await _httpClient.GetStringAsync(url);
                return JsonSerializer.Deserialize<List<Deal>>(json) ?? new List<Deal>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении лучших сделок: {ex.Message}");
                return new List<Deal>();
            }
        }

        public string GetStoreName(string storeId)
        {
            return _stores.TryGetValue(storeId, out var store) ? store.StoreName : "Неизвестный магазин";
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    public class GameDeal
    {
        [JsonPropertyName("price")]
        public string NormalPrice { get; set; } = "";

        [JsonPropertyName("storeID")]
        public string StoreID { get; set; } = "";

        [JsonPropertyName("dealID")]
        public string DealID { get; set; } = "";

        [JsonPropertyName("retailPrice")]
        public string SalePrice { get; set; } = "";

        [JsonPropertyName("savings")]
        public string Savings { get; set; } = "";
        public decimal SalePriceDecimal => decimal.TryParse(SalePrice, out var price) ? price : 0;
        public decimal SavingsDecimal => decimal.TryParse(Savings, out var savings) ? savings : 0;
    }
    public class GameInfo
    {

        [JsonPropertyName("deals")]
        public List<GameDeal> Deals { get; set; } = new();
    }
}

using CheapSharkClient.Models;
using CheapSharkClient.Services;

namespace CheapSharkClient
{
    class Program
    {
        private static CheapSharkService _service = new();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать в CheapShark Client!");
            Console.WriteLine("Здесь вы можете найти лучшие цены на игры!\n");

            while (true)
            {
                ShowMenu();
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await SearchGames();
                        break;
                    case "2":
                        await ShowBestDeals();
                        break;
                    case "3":
                        await CompareGamePrices();
                        break;
                    case "q":
                        Console.WriteLine("До свидания!");
                        _service.Dispose();
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.\n");
                        break;
                }
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1 -   Поиск игр");
            Console.WriteLine("2 -   Лучшие сделки");
            Console.WriteLine("3 -   Сравнить цены на игру");
            Console.WriteLine("q -   Выход");
            Console.WriteLine("═══════════════════════════════════════");
            Console.Write("Ваш выбор: ");
        }

        static async Task SearchGames()
        {
            Console.Write("\nВведите название игры: ");
            string? keyword = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(keyword))
            {
                Console.WriteLine("Название не может быть пустым!\n");
                return;
            }

            var games = await _service.SearchGamesAsync(keyword);
            
            if (games.Count == 0)
            {
                Console.WriteLine("Ничего не найдено\n");
                return;
            }

            Console.WriteLine($"\nНайдено игр: {games.Count}");
            Console.WriteLine("─────────────────────────────────────");
            
            for (int i = 0; i < games.Count; i++)
            {
                var game = games[i];
                Console.WriteLine($"{i + 1}. {game.External}");
                Console.WriteLine($"Лучшая цена: ${game.Cheapest}");
                Console.WriteLine();
            }
        }

        static async Task ShowBestDeals()
        {
            Console.WriteLine("\nЗагружаем лучшие сделки...");
            var deals = await _service.GetBestDealsAsync(15);
            
            if (deals.Count == 0)
            {
                Console.WriteLine("Сделки не найдены\n");
                return;
            }

            var sortedDeals = deals.OrderByDescending(d => d.SavingsDecimal).ToList();
            
            Console.WriteLine("\nТОП-15 лучших сделок (по размеру скидки):");
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            
            for (int i = 0; i < Math.Min(15, sortedDeals.Count); i++)
            {
                var deal = sortedDeals[i];
                string storeName = _service.GetStoreName(deal.StoreID);
                
                Console.WriteLine($"{i + 1:D2}. {deal.Title}");
                Console.WriteLine($"       Магазин: {storeName}");
                Console.WriteLine($"       Цена: ${deal.SalePrice} (было ${deal.NormalPrice})");
                Console.WriteLine($"       Скидка: {deal.Savings:F1}%");
                Console.WriteLine();
            }
        }

        static async Task CompareGamePrices()
        {
            Console.Write("\nВведите название игры для сравнения цен: ");
            string? keyword = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(keyword))
            {
                Console.WriteLine("Название не может быть пустым!\n");
                return;
            }

            var games = await _service.SearchGamesAsync(keyword, 5);
            
            if (games.Count == 0)
            {
                Console.WriteLine("Игры не найдены\n");
                return;
            }

            Console.WriteLine("\nВыберите игру:");
            for (int i = 0; i < games.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {games[i].External} (${games[i].Cheapest})");
            }

            Console.Write("Номер игры: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= games.Count)
            {
                var selectedGame = games[choice - 1];
                var deals = await _service.GetDealsAsync(selectedGame.GameID);
                
                if (deals.Count == 0)
                {
                    Console.WriteLine("Сделки для этой игры не найдены\n");
                    return;
                }

                var sortedDeals = deals.OrderBy(d => d.SalePriceDecimal).ToList();
                
                Console.WriteLine($"\nСравнение цен для '{selectedGame.External}':");
                Console.WriteLine("═══════════════════════════════════════════════════════");
                
                for (int i = 0; i < sortedDeals.Count; i++)
                {
                    var deal = sortedDeals[i];
                    string storeName = _service.GetStoreName(deal.StoreID);
                    string priceIndicator = i == 0 ? "ЛУЧШАЯ ЦЕНА!" : "";
                    
                    Console.WriteLine($"{i + 1:D2}. {storeName} {priceIndicator}");
                    Console.WriteLine($"      Цена: ${deal.SalePrice}");
                    if (deal.SavingsDecimal > 0)
                    {
                        Console.WriteLine($"      Скидка: {deal.Savings:F1}% (было ${deal.NormalPrice})");
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Неверный выбор!\n");
            }
        }
    }
}

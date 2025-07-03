using System.Text.Json;
using System.Text.Json.Serialization;
using CheapSharkClient;

HttpClient httpClient = new HttpClient();

while (true)
{
    Console.Write("Введи название игры или q для выхода\n");
    string? keyword = Console.ReadLine();
    if (keyword == "q")
    {
        break;
    }

    string url = $"https://www.cheapshark.com/api/1.0/games?title={keyword}&limit=5";

    try
    {
        string json = await httpClient.GetStringAsync(url);
        List<Game>? games = JsonSerializer.Deserialize<List<Game>>(json);
        if (games == null || games.Count == 0)
        {
            Console.WriteLine("Ничего не найдено");
        }

        else
        {
            foreach (Game game in games)
            {
                Console.WriteLine($"Название: {game.external}\nЦена: {game.cheapest}$\n");
            }
        }
    }

    catch (Exception ex)
    {
        Console.WriteLine($"Произошла ошибка: {ex.Message}");
    }
}
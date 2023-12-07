using GameAPI.DTOs;
using PokerLibrary.Models;
using Project8.FileManager;

namespace GameAPI.GameManagerD;

public class GameManager : IListManager<Dictionary<string, GameDTO>>
{
    public static Dictionary<string, GameDTO> Games = new();
    private static event FileManager<Dictionary<string, GameDTO>>.SaveToFileDelegate? SaveNewList;
    private static string FileName = "Games.json";
    private static readonly FileManager<Dictionary<string, GameDTO>> fileManager = new();

    private static object _istanceLock = new();
    private static GameManager? _instance = null;

    public static GameManager ScoreboardManagerInstance
    {
        get
        {
            if (_instance == null)
            {
                lock (_istanceLock)
                {
                    _instance ??= new GameManager();
                }
            }
            return _instance;
        }
    }

    public GameManager()
    {
        LoadList();
        Console.WriteLine($"Loaded {Games.Count()} Games saved");

        SaveNewList += SaveToFile;
    }

    public async void LoadList()
    {
        Games = await fileManager.LoadFromFile(FileName) ?? new();
    }

    public void SaveToFile(string filename, Dictionary<string, GameDTO> game)
    {
        fileManager.SaveToFile(filename, game);
    }

    public void UpdateJsons()
    {
        SaveNewList?.Invoke(FileName, Games);
    }

    public GameDTO? FindGame(string email)
    {
        Games.TryGetValue(email, out GameDTO? game);
        return game;
    }

    public bool AddGameToList(GameDTO gameDTO)
    {
        if (Games.TryAdd(gameDTO.Players[0].Email, gameDTO))
        {
            return true;
        }
        return false;
    }
}

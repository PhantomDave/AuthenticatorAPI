using GameAPI.DTOs;
using PokerLibrary.Models;
using Project8.FileManager;

namespace GameAPI.GameManagerD;

public class GameManager : IListManager<Dictionary<string, Game>>
{
    public static Dictionary<string, Game> Games = new();
    private static event FileManager<Dictionary<string, Game>>.SaveToFileDelegate? SaveNewList;
    private static string _fileName = "Games.json";
    private static readonly FileManager<Dictionary<string, Game>> FileManager = new();

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
        Games = await FileManager.LoadFromFile(_fileName) ?? new();
    }

    public void SaveToFile(string filename, Dictionary<string, Game> game)
    {
        FileManager.SaveToFile(filename, game);
    }

    public void UpdateJsons()
    {
        SaveNewList?.Invoke(_fileName, Games);
    }

    public Game? FindGame(string email)
    {
        Games.TryGetValue(email, out Game? game);
        return game;
    }

    public bool AddGameToList(Game gameDto)
    {
        if (Games.TryAdd(gameDto.Players[0].Email, gameDto))
        {
            UpdateJsons();
            return true;
        }
        return false;
    }

    
    public bool RemoveGameToList(string mail) 
    {
        if(Games.Remove(mail)) 
        {
            UpdateJsons();
            return true;
        }
        return false;
    }
}

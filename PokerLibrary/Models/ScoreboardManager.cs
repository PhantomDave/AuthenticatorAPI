using Project8.FileManager;

namespace PokerLibrary.Models;

public class ScoreboardManager : IListManager<List<Scoreboard>>
{
    public static List<Scoreboard> ScoreboardList = new();
    private static event FileManager<List<Scoreboard>>.SaveToFileDelegate? SaveNewList;
    private static readonly string FileName = "Scoresboard.json";
    private static readonly FileManager<List<Scoreboard>> fileManager = new();
    private static object _istanceLock = new();
    private static ScoreboardManager? _instance = null;

    public static ScoreboardManager ScoreboardManagerInstance
    {
        get
        {
            if (_instance == null)
            {
                lock (_istanceLock)
                {
                    _instance ??= new ScoreboardManager();
                }
            }
            return _instance;
        }
    }

    public ScoreboardManager()
    {
        LoadList();
        Console.WriteLine($"Loaded {ScoreboardList.Count()} Scores saved");

        SaveNewList += SaveToFile;
    }

    public async void LoadList()
    {
        ScoreboardList = await fileManager.LoadFromFile(FileName) ?? new();
    }

    public static void SaveToFile(string filename, List<Scoreboard> scoreboard)
    {
        fileManager.SaveToFile(filename, scoreboard);
    }

    public void UpdateJsons()
    {
        SaveNewList?.Invoke(FileName, ScoreboardList);
    }

    public void AddToList(Scoreboard scoreboard)
    {
        Scoreboard? sc = ScoreboardList.FirstOrDefault(s => s.Email == scoreboard.Email);
        Console.WriteLine($"ADDING TO SCOREBOARD: {scoreboard.Email} {sc is null}");
        if (sc == null)
        {
            ScoreboardList.Add(scoreboard);
        }
        else
        {
            sc.SumScores(scoreboard);
        }
        UpdateJsons();
    }
}

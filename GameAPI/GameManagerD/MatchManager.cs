using GameAPI.GameManagerD;
using PokerLibrary.Models;

namespace GameManagerD;

public class MatchManager
{
    public Dictionary<string, Game> Games = new Dictionary<string, Game>();
    private static object _istanceLock = new();
    private static MatchManager? _instance = null;

    public static MatchManager ScoreboardManagerInstance
    {
        get
        {
            if (_instance == null)
            {
                lock (_istanceLock)
                {
                    _instance ??= new MatchManager();
                }
            }
            return _instance;
        }
    }

    public bool AddGameToManager(Game game, string email)
    {
        if (Games.TryAdd(email, game))
        {
            return true;
        }
        return false;
    }

    public Game? GetGame(string email)
    { 
        Games.TryGetValue(email, out Game? game);
        return game;
    }

    public void RemoveGame(string email)
    {
        Games.Remove(email);
    }
}
